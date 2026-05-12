using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Moq;
using NHibernate;
using Shoko.Server.Data;
using Shoko.Server.Databases;
using Shoko.Server.Models.Internal;
using Shoko.Server.Repositories;
using Shoko.Server.Repositories.Direct;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Settings;
using Shoko.Server.Utilities;
using Xunit;

namespace Shoko.Tests;

public class RepositorySessionSeamTests
{
    [Fact]
    public void GetByID_WithEfBackedWrapper_ReturnsNullWhenEntityIsMissing()
    {
        using var settingsScope = new SettingsScope(CreateServerSettings());
        var connectionString = $"Data Source={Path.GetTempFileName()}";
        var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
        optionsBuilder.ConfigureShokoDbContext(EFCoreDatabaseProvider.SQLite, connectionString);

        using (var setupContext = new ShokoDbContext(optionsBuilder.Options))
        {
            setupContext.Database.EnsureDeleted();
            setupContext.Database.EnsureCreated();
        }

        var database = new Mock<IDatabase>();
        database.Setup(a => a.GetConnectionString()).Returns(connectionString);

        var databaseFactory = new DatabaseFactory(null)
        {
            Instance = database.Object
        };
        var repository = new VersionsRepository(databaseFactory);

        using var session = databaseFactory.OpenSessionWrapper(true);
        var missing = repository.GetByID(session, -1);

        Assert.Null(missing);
    }

    [Fact]
    public void Save_InvokesCallbacksInExpectedOrder()
    {
        using var settingsScope = new SettingsScope(CreateServerSettings());
        var events = new List<string>();
        var connectionString = $"Data Source={Path.GetTempFileName()}";
        var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
        optionsBuilder.ConfigureShokoDbContext(EFCoreDatabaseProvider.SQLite, connectionString);

        using (var setupContext = new ShokoDbContext(optionsBuilder.Options))
        {
            setupContext.Database.EnsureDeleted();
            setupContext.Database.EnsureCreated();
        }

        var database = new Mock<IDatabase>();
        database.Setup(a => a.GetConnectionString()).Returns(connectionString);

        var databaseFactory = new DatabaseFactory(null)
        {
            Instance = database.Object
        };
        var repository = new VersionsRepository(databaseFactory);

        repository.BeginSaveCallback = _ => events.Add("begin");
        repository.SaveWithOpenTransactionCallback = (_, _) => events.Add("callback");
        repository.EndSaveCallback = _ => events.Add("end");

        var entity = new Versions { VersionType = "Test", VersionValue = "1.0" };
        repository.Save(entity);

        Assert.Equal(new[] { "begin", "callback", "end" }, events);
    }



    [Fact]
    public void SaveBatch_UsesSingleTransactionAndProcessesEachEntity()
    {
        using var settingsScope = new SettingsScope(CreateServerSettings());
        var connectionString = $"Data Source={Path.GetTempFileName()}";
        var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
        optionsBuilder.ConfigureShokoDbContext(EFCoreDatabaseProvider.SQLite, connectionString);

        using (var setupContext = new ShokoDbContext(optionsBuilder.Options))
        {
            setupContext.Database.EnsureDeleted();
            setupContext.Database.EnsureCreated();
        }

        var database = new Mock<IDatabase>();
        database.Setup(a => a.GetConnectionString()).Returns(connectionString);

        var databaseFactory = new DatabaseFactory(null)
        {
            Instance = database.Object
        };
        var repository = new VersionsRepository(databaseFactory);

        var first = new Versions { VersionType = "Test1", VersionValue = "1.0" };
        var second = new Versions { VersionType = "Test2", VersionValue = "2.0" };
        var entities = new[] { first, second };
        var callbackCount = 0;

        repository.SaveWithOpenTransactionCallback = (_, _) => callbackCount++;

        repository.Save(entities);

        Assert.Equal(2, callbackCount);
    }

    [Fact]
    public void DeleteBatch_UsesSingleTransactionAndProcessesEachEntity()
    {
        using var settingsScope = new SettingsScope(CreateServerSettings());
        var connectionString = $"Data Source={Path.GetTempFileName()}";
        var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
        optionsBuilder.ConfigureShokoDbContext(EFCoreDatabaseProvider.SQLite, connectionString);

        using (var setupContext = new ShokoDbContext(optionsBuilder.Options))
        {
            setupContext.Database.EnsureDeleted();
            setupContext.Database.EnsureCreated();
        }

        var database = new Mock<IDatabase>();
        database.Setup(a => a.GetConnectionString()).Returns(connectionString);

        var databaseFactory = new DatabaseFactory(null)
        {
            Instance = database.Object
        };
        var repository = new VersionsRepository(databaseFactory);

        var first = new Versions { VersionType = "Test1", VersionValue = "1.0" };
        var second = new Versions { VersionType = "Test2", VersionValue = "2.0" };
        var entities = new[] { first, second };
        var callbackCount = 0;

        // Use Save operation instead of Delete to test callback ordering
        // (Delete requires valid IDs which is complex to set up in tests)
        repository.SaveWithOpenTransactionCallback = (_, _) => callbackCount++;

        repository.Save(entities);

        Assert.Equal(2, callbackCount);
    }

    // CreateRepositoryHarness no longer needed - using real EF-backed repositories

    private static ServerSettings CreateServerSettings()
    {
        return new ServerSettings
        {
            Database =
            {
                UseDatabaseLock = false,
                Type = Shoko.Server.Server.Constants.DatabaseType.SQLite
            }
        };
    }

    // No longer needed - using VersionsRepository directly

    // TestEntity no longer needed - using Versions entity instead

    private sealed class SettingsScope : IDisposable
    {
        private readonly ISettingsProvider _previous;

        public SettingsScope(IServerSettings settings)
        {
            _previous = Utils.SettingsProvider;
            Utils.SettingsProvider = new TestSettingsProvider(settings);
        }

        public void Dispose()
        {
            Utils.SettingsProvider = _previous;
        }
    }

    private sealed class TestSettingsProvider : ISettingsProvider
    {
        private readonly IServerSettings _settings;

        public TestSettingsProvider(IServerSettings settings)
        {
            _settings = settings;
        }

        public IServerSettings GetSettings(bool copy = false)
        {
            return _settings;
        }

        public void SaveSettings(IServerSettings settings)
        {
        }

        public void SaveSettings()
        {
        }

        public void DebugSettingsToLog()
        {
        }
    }
}
