using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shoko.Server.Data;
using Shoko.Server.Databases;
using Shoko.Server.Settings;

namespace Shoko.Server.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection AddShokoDbContext(this IServiceCollection services)
    {
        services.AddDbContext<ShokoDbContext>((provider, options) =>
        {
            var settingsProvider = provider.GetRequiredService<ISettingsProvider>();
            var settings = settingsProvider.GetSettings();
            var databaseType = settings.Database.Type;
            
            // Get DatabaseFactory instance from service provider
            var databaseFactory = provider.GetRequiredService<DatabaseFactory>();
            var connectionString = databaseFactory.Instance.GetConnectionString();
            
            // Provider selection logic mirroring DatabaseFactory.Instance
            var providerType = databaseType switch
            {
                Shoko.Server.Server.Constants.DatabaseType.SQLite => EFCoreDatabaseProvider.SQLite,
                Shoko.Server.Server.Constants.DatabaseType.MySQL => EFCoreDatabaseProvider.MariaDB,
                Shoko.Server.Server.Constants.DatabaseType.SQLServer => EFCoreDatabaseProvider.SQLServer,
                _ => EFCoreDatabaseProvider.SQLite // Default to SQLite
            };
            
            options.ConfigureShokoDbContext(providerType, connectionString);
        });
        
        return services;
    }
}