using System;
using System.Collections.Generic;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NHibernate.AdoNet;
using NHibernate.Driver;
using NHibernate.Driver.MySqlConnector;
using Shoko.Server.Data;
using Shoko.Server.Databases.SqliteFixes;
using Shoko.Server.Services;
using Shoko.Server.Utilities;

namespace Benchmarks.T172;

public sealed class BenchmarkDatabaseHarness : IDisposable
{
    private readonly BenchmarkHarnessSettings _settings;
    private readonly DbContextOptions<ShokoDbContext> _efOptions;
    private readonly ISessionFactory _sessionFactory;

    public BenchmarkDatabaseHarness(BenchmarkHarnessSettings settings)
    {
        _settings = settings;
        _settings.ValidateForDatabaseBenchmarks();
        BenchmarkBootstrapper.Initialize(settings);
        _efOptions = CreateEfOptions(settings);
        _sessionFactory = CreateSessionFactory(settings);
    }

    public IReadOnlyList<string> ScenarioIds => _settings.ResolveScenarioIds();

    public int ExecuteEfScenario(string scenarioId)
    {
        var scenario = BenchmarkScenarioRegistry.Get(scenarioId);
        using var context = new ShokoDbContext(_efOptions);
        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        return scenario.ExecuteEf(context);
    }

    public int ExecuteNhScenario(string scenarioId)
    {
        var scenario = BenchmarkScenarioRegistry.Get(scenarioId);
        using var session = _sessionFactory.OpenSession();
        session.DefaultReadOnly = true;
        return scenario.ExecuteNh(session);
    }

    public IReadOnlyList<BenchmarkDryRunResult> RunDry()
    {
        var results = new List<BenchmarkDryRunResult>();
        foreach (var scenarioId in ScenarioIds)
        {
            if (_settings.Mode is BenchmarkMode.EFCore or BenchmarkMode.Both)
            {
                results.Add(new BenchmarkDryRunResult(scenarioId, BenchmarkMode.EFCore, ExecuteEfScenario(scenarioId)));
            }

            if (_settings.Mode is BenchmarkMode.NHibernate or BenchmarkMode.Both)
            {
                results.Add(new BenchmarkDryRunResult(scenarioId, BenchmarkMode.NHibernate, ExecuteNhScenario(scenarioId)));
            }
        }

        return results;
    }

    public void Dispose()
    {
        _sessionFactory.Dispose();
    }

    private static DbContextOptions<ShokoDbContext> CreateEfOptions(BenchmarkHarnessSettings settings)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
        optionsBuilder.ConfigureShokoDbContext(settings.Provider switch
        {
            BenchmarkProviderType.SQLite => EFCoreDatabaseProvider.SQLite,
            BenchmarkProviderType.MariaDB => EFCoreDatabaseProvider.MariaDB,
            BenchmarkProviderType.SQLServer => EFCoreDatabaseProvider.SQLServer,
            _ => EFCoreDatabaseProvider.SQLite,
        }, settings.ConnectionString);
        return optionsBuilder.Options;
    }

    private static ISessionFactory CreateSessionFactory(BenchmarkHarnessSettings settings)
    {
        var serviceProvider = Utils.ServiceContainer ?? new ServiceCollection().BuildServiceProvider();
        var fluent = Fluently.Configure()
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<SystemService>())
            .ExposeConfiguration(c => c.SetInterceptor(new BenchmarkNhInterceptor(serviceProvider)));

        fluent = settings.Provider switch
        {
            BenchmarkProviderType.SQLite => fluent.Database(MsSqliteConfiguration.Standard
                .ConnectionString(c => c.Is(settings.ConnectionString))
                .Dialect<SqliteDialectFix>()
                .Driver<SqliteDriverFix>()),
            BenchmarkProviderType.MariaDB => fluent.Database(MySQLConfiguration.Standard
                .Driver<MySqlConnectorDriver>()
                .ConnectionString(settings.ConnectionString)),
            BenchmarkProviderType.SQLServer => fluent.Database(MsSqlConfiguration.MsSql2012
                .ConnectionString(settings.ConnectionString)
                .Driver<MicrosoftDataSqlClientDriver>()),
            _ => throw new InvalidOperationException($"Unsupported benchmark provider {settings.Provider}"),
        };

        if (settings.Provider == BenchmarkProviderType.SQLServer)
        {
            fluent = fluent.ExposeConfiguration(c => c.DataBaseIntegration(prop =>
            {
                prop.Batcher<NonBatchingBatcherFactory>();
                prop.BatchSize = 0;
            }));
        }

        return fluent.BuildSessionFactory();
    }
}

public sealed record BenchmarkDryRunResult(string ScenarioId, BenchmarkMode Mode, int ResultCount);
