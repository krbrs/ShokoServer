using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Data;
using Shoko.Server.Data.SchemaComparison;
using Shoko.Server.Utilities;

namespace Shoko.IntegrationTests;

/// <summary>
/// Verifies that all database migrations run without error against the backend
/// configured via environment variables (see <see cref="DatabaseMigrationFixture"/>).
/// </summary>
[Collection("Database")]
public class DatabaseMigrationTests : IClassFixture<DatabaseMigrationFixture>
{
    private readonly DatabaseMigrationFixture _fixture;

    public DatabaseMigrationTests(DatabaseMigrationFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void MigrationsCompleteSuccessfully()
    {
        Assert.True(_fixture.Success, _fixture.FailureMessage ?? "Database initialization failed");
    }

    [Fact]
    public async Task StartupAutomaticallyActivatesEfBaselineAndLeavesDatabaseIdempotent()
    {
        Assert.True(_fixture.Success, _fixture.FailureMessage ?? "Database initialization failed");

        string initialCreateMigrationId;
        using (var stateScope = Utils.ServiceContainer.CreateScope())
        {
            var stateContext = stateScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            initialCreateMigrationId = stateContext.Database.GetMigrations().Single(migrationId => migrationId.EndsWith("_InitialCreate", StringComparison.Ordinal));
        }

        using (var verifyScope = Utils.ServiceContainer.CreateScope())
        {
            var verifyContext = verifyScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var preActivationComparison = await new SchemaComparer(verifyContext).CompareAsync();
            Assert.True(preActivationComparison.IsValid,
                $"Startup schema comparison failed:{Environment.NewLine}{string.Join(Environment.NewLine, preActivationComparison.Errors.Select(error => error.Message))}");
        }

        using (var activationScope = Utils.ServiceContainer.CreateScope())
        {
            var activationContext = activationScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var activationService = new EfStartupActivationService(activationContext);
            var activationResult = await activationService.ActivateAsync();
            Assert.True(activationResult.Success, string.Join(Environment.NewLine, activationResult.Errors));
        }

        using (var historyScope = Utils.ServiceContainer.CreateScope())
        {
            var historyContext = historyScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            await historyContext.Database.OpenConnectionAsync();
            await using var command = historyContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = $"SELECT COUNT(*) FROM __EFMigrationsHistory WHERE MigrationId = '{initialCreateMigrationId}'";
            var registrationCount = Convert.ToInt32(await command.ExecuteScalarAsync());
            Assert.Equal(1, registrationCount);
        }
    }
}
