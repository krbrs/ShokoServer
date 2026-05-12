using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

#nullable enable

namespace Shoko.Server.Data.SchemaComparison;

public class EfStartupActivationResult
{
    public bool Success { get; set; }

    public string BaselineMigrationId { get; set; } = string.Empty;

    public List<string> AppliedMigrations { get; set; } = [];

    public BaselineRegistrationResult? BaselineRegistration { get; set; }

    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];
}

public class EfStartupActivationService
{
    private readonly ShokoDbContext _context;

    public EfStartupActivationService(ShokoDbContext context)
    {
        _context = context;
    }

    public async Task<EfStartupActivationResult> ActivateAsync(CancellationToken cancellationToken = default)
    {
        var result = new EfStartupActivationResult();
        var allMigrations = _context.Database.GetMigrations().ToList();

        if (allMigrations.Count == 0)
        {
            result.Success = true;
            result.Warnings.Add("No EF Core migrations are defined. Startup activation skipped.");
            return result;
        }

        var baselineMigrationId = allMigrations[0];
        result.BaselineMigrationId = baselineMigrationId;

        var baselineRegistration = new BaselineRegistration(_context, baselineMigrationId, GetEfProductVersion());
        var baselineResult = await baselineRegistration.RegisterBaselineAsync();
        result.BaselineRegistration = baselineResult;
        result.Warnings.AddRange(baselineResult.Warnings);

        if (!baselineResult.Success)
        {
            result.Errors.AddRange(baselineResult.Errors);
            return result;
        }

        cancellationToken.ThrowIfCancellationRequested();
        var pendingMigrations = _context.Database.GetPendingMigrations().ToList();
        if (pendingMigrations.Count > 0)
        {
            await _context.Database.MigrateAsync(cancellationToken);
            result.AppliedMigrations.AddRange(pendingMigrations);
        }

        result.Success = true;
        return result;
    }

    private static string GetEfProductVersion()
    {
        var informationalVersion = typeof(DbContext).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;
        if (!string.IsNullOrWhiteSpace(informationalVersion))
        {
            var separatorIndex = informationalVersion.IndexOf('+', StringComparison.Ordinal);
            return separatorIndex > 0 ? informationalVersion[..separatorIndex] : informationalVersion;
        }

        return typeof(DbContext).Assembly.GetName().Version?.ToString(3) ?? "unknown";
    }
}
