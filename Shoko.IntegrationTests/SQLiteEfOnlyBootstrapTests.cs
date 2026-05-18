using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using Quartz;
using Shoko.Abstractions.Core.Services;
using Shoko.Abstractions.Metadata.Anidb.Services;
using Shoko.Abstractions.Video.Events;
using Shoko.Abstractions.Video.Enums;
using Shoko.Abstractions.Video.Services;
using Shoko.Server.Data;
using Shoko.Server.Data.SchemaComparison;
using Shoko.Server.Databases;
using Shoko.Server.Models.Shoko;
using Shoko.Server.Repositories;
using Shoko.Server.Scheduling;
using Shoko.Server.Scheduling.GenericJobBuilder;
using Shoko.Server.Scheduling.Jobs;
using Shoko.Server.Scheduling.Jobs.Shoko;
using Shoko.Server;
using Shoko.Server.Services;
using Shoko.Server.Utilities;
using Xunit;

#nullable enable

namespace Shoko.IntegrationTests;

[Collection("Database")]
public class SQLiteEfOnlyBootstrapTests
{
    private static readonly string[] LongRunNLogClampPatterns =
    [
        "Microsoft.EntityFrameworkCore*",
        "Microsoft.Data.Sqlite*",
        "Command",
        "Connection",
        "Infrastructure",
        "Query"
    ];

    private static int _longRunAppHostLoggingClampDepth;

    private static void ResetEfOnlyTestState()
    {
        SQLite.ResetTestState();
        QuartzExtensions.ResetTestState();
        QuartzStartup.ResetTestState();
    }

    private static void ApplyLongRunAppHostLoggingClamp(Shoko.Server.Settings.IServerSettings settings)
    {
        settings.DumpSettingsOnStart = false;
        UpsertLogRule(settings.Logging.LogLevelRules, "Microsoft.EntityFrameworkCore.*", Microsoft.Extensions.Logging.LogLevel.Warning);
        UpsertLogRule(settings.Logging.LogLevelRules, "Microsoft.EntityFrameworkCore.Database.*", Microsoft.Extensions.Logging.LogLevel.Warning);
        UpsertLogRule(settings.Logging.LogLevelRules, "Microsoft.EntityFrameworkCore.Query.*", Microsoft.Extensions.Logging.LogLevel.Warning);
        UpsertLogRule(settings.Logging.LogLevelRules, "Microsoft.EntityFrameworkCore.Infrastructure", Microsoft.Extensions.Logging.LogLevel.Warning);
        UpsertLogRule(settings.Logging.LogLevelRules, "Microsoft.EntityFrameworkCore.ChangeTracking.*", Microsoft.Extensions.Logging.LogLevel.Warning);
    }

    private static void UpsertLogRule(System.Collections.Generic.List<Shoko.Server.Settings.LogLevelRuleConfiguration> rules, string pattern, Microsoft.Extensions.Logging.LogLevel maxLevel)
    {
        var existing = rules.FirstOrDefault(rule => string.Equals(rule.LoggerNamePattern, pattern, StringComparison.Ordinal));
        if (existing is not null)
        {
            existing.MaxLevel = maxLevel;
            existing.Final = true;
            return;
        }

        rules.Add(new()
        {
            LoggerNamePattern = pattern,
            MaxLevel = maxLevel,
            Final = true
        });
    }

    private static IDisposable BeginLongRunAppHostLoggingClamp()
    {
        Interlocked.Increment(ref _longRunAppHostLoggingClampDepth);
        return new DisposableAction(() =>
        {
            if (Interlocked.Decrement(ref _longRunAppHostLoggingClampDepth) == 0)
                RemoveLongRunAppHostNLogClamp();
        });
    }

    private static void ApplyCurrentLongRunAppHostNLogClampIfRequested()
    {
        if (Volatile.Read(ref _longRunAppHostLoggingClampDepth) <= 0)
            return;

        var config = LogManager.Configuration;
        if (config is null)
            return;

        var voidTarget = config.FindTargetByName("void");
        if (voidTarget is null)
            return;

        var added = false;
        foreach (var pattern in LongRunNLogClampPatterns)
        {
            if (config.LoggingRules.Any(rule =>
                    string.Equals(rule.LoggerNamePattern, pattern, StringComparison.Ordinal) &&
                    rule.Targets.Contains(voidTarget) &&
                    rule.Final))
                continue;

            var rule = new LoggingRule(pattern, NLog.LogLevel.Trace, NLog.LogLevel.Info, voidTarget)
            {
                Final = true
            };
            config.LoggingRules.Insert(0, rule);
            added = true;
        }

        if (added)
            LogManager.ReconfigExistingLoggers();
    }

    private static void RemoveLongRunAppHostNLogClamp()
    {
        var config = LogManager.Configuration;
        if (config is null)
            return;

        var voidTarget = config.FindTargetByName("void");
        if (voidTarget is null)
            return;

        var removed = false;
        for (var i = config.LoggingRules.Count - 1; i >= 0; i--)
        {
            var rule = config.LoggingRules[i];
            if (!rule.Targets.Contains(voidTarget) || !rule.Final)
                continue;
            if (!LongRunNLogClampPatterns.Contains(rule.LoggerNamePattern, StringComparer.Ordinal))
                continue;

            config.LoggingRules.RemoveAt(i);
            removed = true;
        }

        if (removed)
            LogManager.ReconfigExistingLoggers();
    }

    [Fact]
    public async Task SQLite_EfOnlyBootstrap_PopulatesInitialData_And_SurvivesRestart()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"shoko-efonly-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        var succeeded = false;

        var originalShokoHome = Environment.GetEnvironmentVariable("SHOKO_HOME");
        try
        {
            Environment.SetEnvironmentVariable("SHOKO_HOME", tempDir.Replace('\\', '/'));
            SQLite.UseEfOnlyBootstrapForTests = true;
            ResetEfOnlyTestState();
            RepoFactory.ResetTestCounters();
            SQLite.ThrowOnSessionFactoryCreateForTests = true;
            Assert.Equal(0, RepoFactory.EfOnlyPopulateSessionCount);
            Assert.Equal(0, RepoFactory.EfOnlySkippedRepairPassCount);
            Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);

            var firstHost = await StartServiceAsync(waitForStartupComplete: true);
            try
            {
                Assert.True(RepoFactory.EfOnlyPopulateSessionCount > 0);
                Assert.Equal(0, RepoFactory.EfOnlySkippedRepairPassCount);
                Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);
                Assert.NotNull(RepoFactory.JMMUser.GetByUsername("Default"));
                Assert.NotEmpty(RepoFactory.FilterPreset.GetAll());

                using (var scope = Utils.ServiceContainer.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
                    var activationService = new EfStartupActivationService(context);
                    var activationResult = await activationService.ActivateAsync();

                    Assert.True(activationResult.Success, string.Join(Environment.NewLine, activationResult.Errors));
                    Assert.Empty(activationResult.AppliedMigrations);
                }
            }
            finally
            {
                    await StopHostAndDrainAsync(firstHost);
            }

            RepoFactory.ResetTestCounters();
            var secondHost = await StartServiceAsync(waitForStartupComplete: true);
            try
            {
                Assert.True(RepoFactory.EfOnlyPopulateSessionCount > 0);
                Assert.Equal(0, RepoFactory.EfOnlySkippedRepairPassCount);
                Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);
                Assert.NotNull(RepoFactory.JMMUser.GetByUsername("Default"));
                Assert.NotEmpty(RepoFactory.FilterPreset.GetAll());
            }
            finally
            {
                    await StopHostAndDrainAsync(secondHost);
            }

            succeeded = true;
        }
        finally
        {
            ResetEfOnlyTestState();
            SQLite.UseEfOnlyBootstrapForTests = false;
            RepoFactory.ResetTestCounters();
            Environment.SetEnvironmentVariable("SHOKO_HOME", originalShokoHome);

            if (succeeded && Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task SQLite_EfOnlyBootstrap_ExistingDatabase_BaselinedStartup_SurvivesRestart_WithoutNhSessionFactory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"shoko-efonly-existing-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        var succeeded = false;

        var originalShokoHome = Environment.GetEnvironmentVariable("SHOKO_HOME");
        try
        {
            Environment.SetEnvironmentVariable("SHOKO_HOME", tempDir.Replace('\\', '/'));
            var databasePath = GetExpectedSqliteDatabasePath(tempDir);
            CopyExistingSqliteFixtureDatabase(databasePath);
            Assert.True(File.Exists(databasePath), $"Expected existing SQLite fixture database at '{databasePath}'.");
            Assert.False(await EfMigrationsHistoryExistsAsync(databasePath), "Expected the existing SQLite fixture to be pre-EF and have no __EFMigrationsHistory table.");

            SQLite.UseEfOnlyBootstrapForTests = true;
            ResetEfOnlyTestState();
            RepoFactory.ResetTestCounters();
            SQLite.ThrowOnSessionFactoryCreateForTests = true;
            Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);
            Assert.Equal(0, RepoFactory.EfOnlyPopulateSessionCount);
            Assert.Equal(0, RepoFactory.EfOnlySkippedRepairPassCount);

            var firstHost = await StartServiceAsync(waitForStartupComplete: true);
            try
            {
                Assert.True(RepoFactory.EfOnlyPopulateSessionCount > 0);
                Assert.Equal(0, RepoFactory.EfOnlySkippedRepairPassCount);
                Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);
                Assert.NotNull(RepoFactory.JMMUser.GetByUsername("Default"));
                Assert.NotEmpty(RepoFactory.FilterPreset.GetAll());

                using var scope = Utils.ServiceContainer.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
                var activationService = new EfStartupActivationService(context);
                var activationResult = await activationService.ActivateAsync();

                Assert.True(activationResult.Success, string.Join(Environment.NewLine, activationResult.Errors));
                Assert.NotNull(activationResult.BaselineRegistration);
                Assert.True(activationResult.BaselineRegistration!.Success, string.Join(Environment.NewLine, activationResult.BaselineRegistration.Errors));
                Assert.Equal(activationResult.BaselineMigrationId, activationResult.BaselineRegistration.RegisteredMigrationId);
                Assert.Empty(activationResult.AppliedMigrations);
            }
            finally
            {
                await StopHostAndDrainAsync(firstHost);
            }

            Assert.True(await EfMigrationsHistoryExistsAsync(databasePath), "Expected EF migration history to exist after EF-only startup activation.");

            ResetEfOnlyTestState();
            RepoFactory.ResetTestCounters();
            SQLite.ThrowOnSessionFactoryCreateForTests = true;

            var secondHost = await StartServiceAsync(waitForStartupComplete: true);
            try
            {
                Assert.True(RepoFactory.EfOnlyPopulateSessionCount > 0);
                Assert.Equal(0, RepoFactory.EfOnlySkippedRepairPassCount);
                Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);
                Assert.NotNull(RepoFactory.JMMUser.GetByUsername("Default"));
                Assert.NotEmpty(RepoFactory.FilterPreset.GetAll());
            }
            finally
            {
                await StopHostAndDrainAsync(secondHost);
            }

            succeeded = true;
        }
        finally
        {
            ResetEfOnlyTestState();
            SQLite.UseEfOnlyBootstrapForTests = false;
            RepoFactory.ResetTestCounters();
            Environment.SetEnvironmentVariable("SHOKO_HOME", originalShokoHome);

            if (succeeded && Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task SQLite_ExistingFixture_TmdbEpisodeWithNullThumbnail_MaterializesWithEf()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"shoko-existing-fixture-materialize-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        try
        {
            var databasePath = GetExpectedSqliteDatabasePath(tempDir);
            CopyExistingSqliteFixtureDatabase(databasePath);

            var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
            optionsBuilder.UseSqlite($"Data Source={databasePath}");

            await using var context = new ShokoDbContext(optionsBuilder.Options);
            var episode = await context.TMDB_Episode
                .AsNoTracking()
                .Where(entry => entry.ThumbnailPath == null)
                .OrderBy(entry => entry.TMDB_EpisodeID)
                .FirstOrDefaultAsync();

            Assert.NotNull(episode);
            Assert.Null(episode!.ThumbnailPath);
            Assert.False(string.IsNullOrEmpty(episode.EnglishTitle));
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task SQLite_ExistingFixture_TmdbImageEntityWithPersonType_MaterializesWithEf()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"shoko-existing-image-entity-materialize-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        try
        {
            var databasePath = GetExpectedSqliteDatabasePath(tempDir);
            CopyExistingSqliteFixtureDatabase(databasePath);

            var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
            optionsBuilder.UseSqlite($"Data Source={databasePath}");

            await using var context = new ShokoDbContext(optionsBuilder.Options);
            var imageEntity = await context.TMDB_Image_Entity
                .AsNoTracking()
                .Where(entry => entry.TmdbEntityType == Shoko.Server.Server.ForeignEntityType.Person)
                .OrderBy(entry => entry.TMDB_Image_EntityID)
                .FirstOrDefaultAsync();

            Assert.NotNull(imageEntity);
            Assert.Equal(Shoko.Server.Server.ForeignEntityType.Person, imageEntity!.TmdbEntityType);
            Assert.False(string.IsNullOrEmpty(imageEntity.RemoteFileName));
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ReachesHashBoundaryWithoutNhSessionFactory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"shoko-efonly-existing-runonstart-{Guid.NewGuid():N}");
        var importDir = Path.Combine(tempDir, "import");
        Directory.CreateDirectory(importDir);
        var relativePath = "existing-runtime-hashchain.mkv";
        var absolutePath = Path.Combine(importDir, relativePath);
        await File.WriteAllBytesAsync(absolutePath, new byte[4096]);

        var originalShokoHome = Environment.GetEnvironmentVariable("SHOKO_HOME");
        using var loggingClamp = BeginLongRunAppHostLoggingClamp();
        try
        {
            Environment.SetEnvironmentVariable("SHOKO_HOME", tempDir.Replace('\\', '/'));
            var databasePath = GetExpectedSqliteDatabasePath(tempDir);
            CopyExistingSqliteFixtureDatabase(databasePath);
            Assert.False(await EfMigrationsHistoryExistsAsync(databasePath), "Expected the existing SQLite fixture to be pre-EF and have no __EFMigrationsHistory table.");

            SQLite.UseEfOnlyBootstrapForTests = true;
            ResetEfOnlyTestState();
            RepoFactory.ResetTestCounters();
            SQLite.ThrowOnSessionFactoryCreateForTests = true;

            WriteTestProgress(nameof(SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ReachesHashBoundaryWithoutNhSessionFactory), "Starting seed host.");
            int folderId;
            {
                var seedHost = await StartServiceAsync(
                    waitForStartupComplete: true,
                    configureSettings: ApplyLongRunAppHostLoggingClamp);
                try
                {
                    Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);

                    foreach (var existingFolder in RepoFactory.ShokoManagedFolder.GetAll())
                    {
                        if (!existingFolder.IsWatched)
                            continue;

                        existingFolder.IsWatched = false;
                        RepoFactory.ShokoManagedFolder.Save(existingFolder);
                    }

                    var folder = new ShokoManagedFolder
                    {
                        Name = $"ExistingRuntime-{Guid.NewGuid():N}",
                        Path = importDir,
                        IsWatched = true
                    };
                    RepoFactory.ShokoManagedFolder.Save(folder);
                    folderId = folder.ID;
                }
                finally
                {
                    WriteTestProgress(nameof(SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ReachesHashBoundaryWithoutNhSessionFactory), "Stopping seed host.");
                    await StopHostAndDrainAsync(seedHost);
                }
            }

            Assert.True(await EfMigrationsHistoryExistsAsync(databasePath), "Expected EF migration history to exist after the first existing-db startup.");
            Assert.True(await EfMigrationsHistoryContainsMigrationAsync(databasePath, "20260509114039_InitialCreate"), "Expected the baseline migration row to persist after the first existing-db startup.");

            ResetEfOnlyTestState();
            RepoFactory.ResetTestCounters();
            SQLite.ThrowOnSessionFactoryCreateForTests = true;

            WriteTestProgress(nameof(SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ReachesHashBoundaryWithoutNhSessionFactory), "Starting main host.");
            var (host, systemService, _) = await StartServiceUntilAboutToStartAsync(
                waitForStartupComplete: false,
                configureSettings: settings =>
                {
                    ApplyLongRunAppHostLoggingClamp(settings);
                    settings.Import.RunOnStart = true;
                    settings.Import.ScanDropFoldersOnStart = false;
                    settings.Import.FileLockChecking = false;
                    settings.Import.AggressiveFileLockChecking = false;
                });

            var queueStateEventHandler = Utils.ServiceContainer.GetRequiredService<QueueStateEventHandler>();
            var observedHashJobForFile = false;
            EventHandler<QueueItemsAddedEventArgs> onQueueItemsAdded = (_, e) =>
            {
                foreach (var item in e.AddedItems)
                {
                    observedHashJobForFile |= IsHashJobForFile(item, relativePath);
                }
            };
            EventHandler<QueueChangedEventArgs> onQueueChanged = (_, e) =>
            {
                foreach (var item in e.AddedItems)
                {
                    observedHashJobForFile |= IsHashJobForFile(item, relativePath);
                }

                foreach (var item in e.ExecutingItems)
                {
                    observedHashJobForFile |= IsHashJobForFile(item, relativePath);
                }
            };
            queueStateEventHandler.QueueItemsAdded += onQueueItemsAdded;
            queueStateEventHandler.ExecutingJobsChanged += onQueueChanged;

            try
            {
                WriteTestProgress(nameof(SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ReachesHashBoundaryWithoutNhSessionFactory), "Waiting for startup.");
                await systemService.WaitForStartupAsync().WaitAsync(TimeSpan.FromMinutes(10));
                WriteTestProgress(nameof(SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ReachesHashBoundaryWithoutNhSessionFactory), "Startup completed.");
                WriteTestProgress(nameof(SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ReachesHashBoundaryWithoutNhSessionFactory), $"Waiting for hash boundary for '{relativePath}'.");
                var hashReached = await WaitForHashBoundaryOrObservedAsync(absolutePath, () => observedHashJobForFile, TimeSpan.FromSeconds(90));
                WriteTestProgress(nameof(SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ReachesHashBoundaryWithoutNhSessionFactory), $"Hash boundary wait completed for '{relativePath}' with result={hashReached}.");

                Assert.True(hashReached, "Expected the existing-db RunOnStart path to reach or observe the hash stage.");
                Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);
            }
            finally
            {
                queueStateEventHandler.QueueItemsAdded -= onQueueItemsAdded;
                queueStateEventHandler.ExecutingJobsChanged -= onQueueChanged;
                WriteTestProgress(nameof(SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ReachesHashBoundaryWithoutNhSessionFactory), "Stopping main host.");
                await StopHostAndDrainAsync(host);
            }
        }
        finally
        {
            ResetEfOnlyTestState();
            SQLite.UseEfOnlyBootstrapForTests = false;
            RepoFactory.ResetTestCounters();
            Environment.SetEnvironmentVariable("SHOKO_HOME", originalShokoHome);
            await DeleteDirectoryWithRetriesAsync(tempDir);
        }
    }

    [Fact]
    public async Task SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ValidVideo_ReachesProcessBoundaryWithoutNhSessionFactory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"shoko-efonly-existing-validvideo-{Guid.NewGuid():N}");
        var importDir = Path.Combine(tempDir, "import");
        Directory.CreateDirectory(importDir);
        var relativePath = "existing-runtime-processchain.mp4";
        var absolutePath = Path.Combine(importDir, relativePath);
        await WriteTinyValidVideoFixtureAsync(absolutePath);

        var originalShokoHome = Environment.GetEnvironmentVariable("SHOKO_HOME");
        using var loggingClamp = BeginLongRunAppHostLoggingClamp();
        try
        {
            Environment.SetEnvironmentVariable("SHOKO_HOME", tempDir.Replace('\\', '/'));
            var databasePath = GetExpectedSqliteDatabasePath(tempDir);
            CopyExistingSqliteFixtureDatabase(databasePath);
            Assert.False(await EfMigrationsHistoryExistsAsync(databasePath), "Expected the existing SQLite fixture to be pre-EF and have no __EFMigrationsHistory table.");

            SQLite.UseEfOnlyBootstrapForTests = true;
            ResetEfOnlyTestState();
            RepoFactory.ResetTestCounters();
            SQLite.ThrowOnSessionFactoryCreateForTests = true;

            WriteTestProgress(nameof(SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ValidVideo_ReachesProcessBoundaryWithoutNhSessionFactory), "Starting seed host.");
            int folderId;
            {
                var seedHost = await StartServiceAsync(
                    waitForStartupComplete: true,
                    configureSettings: ApplyLongRunAppHostLoggingClamp);
                try
                {
                    Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);

                    foreach (var existingFolder in RepoFactory.ShokoManagedFolder.GetAll())
                    {
                        if (!existingFolder.IsWatched)
                            continue;

                        existingFolder.IsWatched = false;
                        RepoFactory.ShokoManagedFolder.Save(existingFolder);
                    }

                    var folder = new ShokoManagedFolder
                    {
                        Name = $"ExistingRuntimeValid-{Guid.NewGuid():N}",
                        Path = importDir,
                        IsWatched = true
                    };
                    RepoFactory.ShokoManagedFolder.Save(folder);
                    folderId = folder.ID;
                }
                finally
                {
                    WriteTestProgress(nameof(SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ValidVideo_ReachesProcessBoundaryWithoutNhSessionFactory), "Stopping seed host.");
                    await StopHostAndDrainAsync(seedHost);
                }
            }

            Assert.True(await EfMigrationsHistoryExistsAsync(databasePath), "Expected EF migration history to exist after the first existing-db startup.");
            Assert.True(await EfMigrationsHistoryContainsMigrationAsync(databasePath, "20260509114039_InitialCreate"), "Expected the baseline migration row to persist after the first existing-db startup.");

            ResetEfOnlyTestState();
            RepoFactory.ResetTestCounters();
            SQLite.ThrowOnSessionFactoryCreateForTests = true;

            WriteTestProgress(nameof(SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ValidVideo_ReachesProcessBoundaryWithoutNhSessionFactory), "Starting main host.");
            var (host, systemService, _) = await StartServiceUntilAboutToStartAsync(
                waitForStartupComplete: false,
                configureSettings: settings =>
                {
                    ApplyLongRunAppHostLoggingClamp(settings);
                    settings.Import.RunOnStart = true;
                    settings.Import.ScanDropFoldersOnStart = false;
                    settings.Import.FileLockChecking = false;
                    settings.Import.AggressiveFileLockChecking = false;
                });

            var queueStateEventHandler = Utils.ServiceContainer.GetRequiredService<QueueStateEventHandler>();
            var observedHashJobForFile = false;
            var observedProcessItems = new ConcurrentBag<Shoko.Server.Scheduling.QueueItem>();
            EventHandler<QueueItemsAddedEventArgs> onQueueItemsAdded = (_, e) =>
            {
                foreach (var item in e.AddedItems)
                {
                    observedHashJobForFile |= IsHashJobForFile(item, relativePath);
                    if (string.Equals(item.JobType, "Get Release Information for Video", StringComparison.Ordinal))
                        observedProcessItems.Add(item);
                }
            };
            EventHandler<QueueChangedEventArgs> onQueueChanged = (_, e) =>
            {
                foreach (var item in e.AddedItems)
                {
                    observedHashJobForFile |= IsHashJobForFile(item, relativePath);
                    if (string.Equals(item.JobType, "Get Release Information for Video", StringComparison.Ordinal))
                        observedProcessItems.Add(item);
                }

                foreach (var item in e.ExecutingItems)
                {
                    observedHashJobForFile |= IsHashJobForFile(item, relativePath);
                    if (string.Equals(item.JobType, "Get Release Information for Video", StringComparison.Ordinal))
                        observedProcessItems.Add(item);
                }
            };
            queueStateEventHandler.QueueItemsAdded += onQueueItemsAdded;
            queueStateEventHandler.ExecutingJobsChanged += onQueueChanged;

            try
            {
                WriteTestProgress(nameof(SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ValidVideo_ReachesProcessBoundaryWithoutNhSessionFactory), "Waiting for startup.");
                await systemService.WaitForStartupAsync().WaitAsync(TimeSpan.FromMinutes(10));
                WriteTestProgress(nameof(SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ValidVideo_ReachesProcessBoundaryWithoutNhSessionFactory), "Startup completed.");
                WriteTestProgress(nameof(SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ValidVideo_ReachesProcessBoundaryWithoutNhSessionFactory), $"Waiting for hashed video boundary for '{relativePath}'.");
                var (videoLocalId, processJobScheduled) = await WaitForHashedVideoOrNhTouchAsync(folderId, relativePath, TimeSpan.FromSeconds(90));
                WriteTestProgress(nameof(SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ValidVideo_ReachesProcessBoundaryWithoutNhSessionFactory), $"Hashed video boundary completed for '{relativePath}' with videoLocalId={videoLocalId}, processJobScheduled={processJobScheduled}.");

                Assert.True(videoLocalId > 0);
                Assert.True(observedHashJobForFile, "Expected the specific HashFileJob to be observed for the valid upgraded-db runtime file.");
                Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);

                WriteTestProgress(nameof(SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ValidVideo_ReachesProcessBoundaryWithoutNhSessionFactory), $"Waiting for process boundary for '{relativePath}'.");
                var reachedProcessBoundary = processJobScheduled || await WaitForProcessJobBoundaryOrObservedAsync(
                    videoLocalId,
                    relativePath,
                    () => observedProcessItems.Any(item => IsProcessJobForFileOrVideo(item, relativePath, videoLocalId)),
                    TimeSpan.FromSeconds(30));
                WriteTestProgress(nameof(SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ValidVideo_ReachesProcessBoundaryWithoutNhSessionFactory), $"Process boundary wait completed for '{relativePath}' with result={reachedProcessBoundary}.");
                Assert.True(reachedProcessBoundary, "Expected the valid upgraded-db runtime file to reach ProcessFileJob scheduling or execution.");
                await AssertProcessFileJobUsesCachedReleaseOfflineAsync(videoLocalId);
            }
            finally
            {
                queueStateEventHandler.QueueItemsAdded -= onQueueItemsAdded;
                queueStateEventHandler.ExecutingJobsChanged -= onQueueChanged;
                WriteTestProgress(nameof(SQLite_EfOnlyBootstrap_ExistingDatabase_RunOnStart_ValidVideo_ReachesProcessBoundaryWithoutNhSessionFactory), "Stopping main host.");
                await StopHostAndDrainAsync(host);
            }
        }
        finally
        {
            ResetEfOnlyTestState();
            SQLite.UseEfOnlyBootstrapForTests = false;
            RepoFactory.ResetTestCounters();
            Environment.SetEnvironmentVariable("SHOKO_HOME", originalShokoHome);
            await DeleteDirectoryWithRetriesAsync(tempDir);
        }
    }

    [Fact]
    public async Task SQLite_EfOnlyBootstrap_RunOnStart_ScansManagedFolderWithoutNhSessionFactory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"shoko-efonly-runonstart-{Guid.NewGuid():N}");
        var importDir = Path.Combine(tempDir, "import");
        Directory.CreateDirectory(importDir);
        var relativePath = "startup-runonstart.mkv";
        await File.WriteAllBytesAsync(Path.Combine(importDir, relativePath), new byte[4096]);

        var originalShokoHome = Environment.GetEnvironmentVariable("SHOKO_HOME");
        try
        {
            Environment.SetEnvironmentVariable("SHOKO_HOME", tempDir.Replace('\\', '/'));
            SQLite.UseEfOnlyBootstrapForTests = true;
            ResetEfOnlyTestState();
            RepoFactory.ResetTestCounters();
            SQLite.ThrowOnSessionFactoryCreateForTests = true;

            var seedHost = await StartServiceAsync(waitForStartupComplete: true);
            int folderId;
            try
            {
                var folder = new ShokoManagedFolder
                {
                    Name = $"RunOnStart-{Guid.NewGuid():N}",
                    Path = importDir,
                    IsWatched = true
                };
                RepoFactory.ShokoManagedFolder.Save(folder);
                folderId = folder.ID;
            }
            finally
            {
                await StopHostAndDrainAsync(seedHost);
            }

            ResetEfOnlyTestState();
            SQLite.ThrowOnSessionFactoryCreateForTests = true;

            var host = await StartServiceAsync(
                waitForStartupComplete: true,
                configureSettings: settings =>
                {
                    settings.Import.RunOnStart = true;
                    settings.Import.ScanDropFoldersOnStart = false;
                    settings.Import.FileLockChecking = false;
                    settings.Import.AggressiveFileLockChecking = false;
                });
            try
            {
                await WaitForManagedFolderPlaceOrNhTouchAsync(folderId, relativePath, TimeSpan.FromSeconds(60));
                Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);
            }
            finally
            {
                await StopHostAndDrainAsync(host);
            }
        }
        finally
        {
            ResetEfOnlyTestState();
            SQLite.UseEfOnlyBootstrapForTests = false;
            RepoFactory.ResetTestCounters();
            Environment.SetEnvironmentVariable("SHOKO_HOME", originalShokoHome);
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task SQLite_EfOnlyBootstrap_ScanDropFoldersOnStart_ScansSourceFolderWithoutNhSessionFactory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"shoko-efonly-scandrop-{Guid.NewGuid():N}");
        var importDir = Path.Combine(tempDir, "drop-source");
        Directory.CreateDirectory(importDir);
        var relativePath = "startup-scandrop.mkv";
        await File.WriteAllBytesAsync(Path.Combine(importDir, relativePath), new byte[4096]);

        var originalShokoHome = Environment.GetEnvironmentVariable("SHOKO_HOME");
        try
        {
            Environment.SetEnvironmentVariable("SHOKO_HOME", tempDir.Replace('\\', '/'));
            SQLite.UseEfOnlyBootstrapForTests = true;
            ResetEfOnlyTestState();
            RepoFactory.ResetTestCounters();
            SQLite.ThrowOnSessionFactoryCreateForTests = true;

            var seedHost = await StartServiceAsync(waitForStartupComplete: true);
            int folderId;
            try
            {
                var folder = new ShokoManagedFolder
                {
                    Name = $"ScanDrop-{Guid.NewGuid():N}",
                    Path = importDir,
                    IsWatched = false,
                    DropFolderType = DropFolderType.Source
                };
                RepoFactory.ShokoManagedFolder.Save(folder);
                folderId = folder.ID;
            }
            finally
            {
                await StopHostAndDrainAsync(seedHost);
            }

            ResetEfOnlyTestState();
            SQLite.ThrowOnSessionFactoryCreateForTests = true;

            var host = await StartServiceAsync(
                waitForStartupComplete: true,
                configureSettings: settings =>
                {
                    settings.Import.RunOnStart = false;
                    settings.Import.ScanDropFoldersOnStart = true;
                    settings.Import.FileLockChecking = false;
                    settings.Import.AggressiveFileLockChecking = false;
                });
            try
            {
                await WaitForManagedFolderPlaceOrNhTouchAsync(folderId, relativePath, TimeSpan.FromSeconds(60));
                Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);
            }
            finally
            {
                await StopHostAndDrainAsync(host);
            }
        }
        finally
        {
            ResetEfOnlyTestState();
            SQLite.UseEfOnlyBootstrapForTests = false;
            RepoFactory.ResetTestCounters();
            Environment.SetEnvironmentVariable("SHOKO_HOME", originalShokoHome);
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task SQLite_EfOnlyBootstrap_RunOnStart_ReachesHashBoundaryWithoutNhSessionFactory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"shoko-efonly-hashchain-{Guid.NewGuid():N}");
        var importDir = Path.Combine(tempDir, "import");
        Directory.CreateDirectory(importDir);
        var relativePath = "startup-hashchain.mkv";
        await File.WriteAllBytesAsync(Path.Combine(importDir, relativePath), new byte[4096]);

        var originalShokoHome = Environment.GetEnvironmentVariable("SHOKO_HOME");
        try
        {
            Environment.SetEnvironmentVariable("SHOKO_HOME", tempDir.Replace('\\', '/'));
            SQLite.UseEfOnlyBootstrapForTests = true;
            ResetEfOnlyTestState();
            RepoFactory.ResetTestCounters();
            SQLite.ThrowOnSessionFactoryCreateForTests = true;

            var seedHost = await StartServiceAsync(waitForStartupComplete: true);
            int folderId;
            try
            {
                var folder = new ShokoManagedFolder
                {
                    Name = $"HashChain-{Guid.NewGuid():N}",
                    Path = importDir,
                    IsWatched = true
                };
                RepoFactory.ShokoManagedFolder.Save(folder);
                folderId = folder.ID;
            }
            finally
            {
                await StopHostAndDrainAsync(seedHost);
            }

            ResetEfOnlyTestState();
            SQLite.ThrowOnSessionFactoryCreateForTests = true;

            var (host, systemService, _) = await StartServiceUntilAboutToStartAsync(
                waitForStartupComplete: false,
                configureSettings: settings =>
                {
                    settings.Import.RunOnStart = true;
                    settings.Import.ScanDropFoldersOnStart = false;
                    settings.Import.FileLockChecking = false;
                    settings.Import.AggressiveFileLockChecking = false;
                });

            var queueStateEventHandler = Utils.ServiceContainer.GetRequiredService<QueueStateEventHandler>();
            var observedHashJob = false;
            var observedProcessJob = false;
            EventHandler<QueueItemsAddedEventArgs> onQueueItemsAdded = (_, e) =>
            {
                foreach (var item in e.AddedItems)
                {
                    observedHashJob |= string.Equals(item.JobType, "Hash File", StringComparison.Ordinal);
                    observedProcessJob |= string.Equals(item.JobType, "Get Release Information for Video", StringComparison.Ordinal);
                }
            };
            EventHandler<QueueChangedEventArgs> onQueueChanged = (_, e) =>
            {
                foreach (var item in e.AddedItems)
                {
                    observedHashJob |= string.Equals(item.JobType, "Hash File", StringComparison.Ordinal);
                    observedProcessJob |= string.Equals(item.JobType, "Get Release Information for Video", StringComparison.Ordinal);
                }

                foreach (var item in e.ExecutingItems)
                {
                    observedHashJob |= string.Equals(item.JobType, "Hash File", StringComparison.Ordinal);
                    observedProcessJob |= string.Equals(item.JobType, "Get Release Information for Video", StringComparison.Ordinal);
                }
            };
            queueStateEventHandler.QueueItemsAdded += onQueueItemsAdded;
            queueStateEventHandler.ExecutingJobsChanged += onQueueChanged;

            try
            {
                await systemService.WaitForStartupAsync().WaitAsync(TimeSpan.FromMinutes(10));
                var (videoLocalId, processJobScheduled) = await WaitForHashedVideoOrNhTouchAsync(folderId, relativePath, TimeSpan.FromSeconds(90));

                Assert.True(videoLocalId > 0);
                Assert.True(observedHashJob, "Expected HashFileJob to be observed during RunOnStart import.");
                Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);

                observedProcessJob |= processJobScheduled;
                Assert.True(observedProcessJob, "Expected ProcessFileJob scheduling to be observed after hashing.");
            }
            finally
            {
                queueStateEventHandler.QueueItemsAdded -= onQueueItemsAdded;
                queueStateEventHandler.ExecutingJobsChanged -= onQueueChanged;
                await StopHostAndDrainAsync(host);
            }
        }
        finally
        {
            ResetEfOnlyTestState();
            SQLite.UseEfOnlyBootstrapForTests = false;
            RepoFactory.ResetTestCounters();
            Environment.SetEnvironmentVariable("SHOKO_HOME", originalShokoHome);
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }

    private static async Task<IHost> StartServiceAsync(bool waitForStartupComplete = false, Action<Shoko.Server.Settings.IServerSettings>? configureSettings = null)
    {
        var (host, _, _) = await StartServiceUntilAboutToStartAsync(waitForStartupComplete, configureSettings);
        return host;
    }

    private static async Task StopHostAndDrainAsync(IHost host)
    {
        WriteTestProgress(nameof(StopHostAndDrainAsync), "Waiting for pending Quartz processing before StopAsync.");
        await QuartzExtensions.WaitForPendingProcessingForTests().WaitAsync(TimeSpan.FromSeconds(30));
        WriteTestProgress(nameof(StopHostAndDrainAsync), "Waiting for pending recurring scheduling before StopAsync.");
        await QuartzStartup.WaitForPendingRecurringSchedulingForTests().WaitAsync(TimeSpan.FromSeconds(30));
        if (Utils.ServiceContainer is not null)
        {
            using var scope = Utils.ServiceContainer.CreateScope();
            var queueHandler = scope.ServiceProvider.GetRequiredService<QueueHandler>();
            WriteTestProgress(nameof(StopHostAndDrainAsync), "Pausing queue before StopAsync.");
            await queueHandler.Pause();
        }
        WriteTestProgress(nameof(StopHostAndDrainAsync), "Calling host.StopAsync.");
        await host.StopAsync(TimeSpan.FromSeconds(30));
        WriteTestProgress(nameof(StopHostAndDrainAsync), "Waiting for pending Quartz processing after StopAsync.");
        await QuartzExtensions.WaitForPendingProcessingForTests().WaitAsync(TimeSpan.FromSeconds(30));
        if (Utils.ServiceContainer is not null)
        {
            using var scope = Utils.ServiceContainer.CreateScope();
            var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
            var scheduler = await schedulerFactory.GetScheduler();
            WriteTestProgress(nameof(StopHostAndDrainAsync), "Shutting down Quartz scheduler explicitly.");
            if (!scheduler.IsShutdown)
                await scheduler.Shutdown(waitForJobsToComplete: true);
        }
        switch (host)
        {
            case IAsyncDisposable asyncDisposable:
                WriteTestProgress(nameof(StopHostAndDrainAsync), "Disposing host asynchronously.");
                await asyncDisposable.DisposeAsync();
                break;
            case IDisposable disposable:
                WriteTestProgress(nameof(StopHostAndDrainAsync), "Disposing host synchronously.");
                disposable.Dispose();
                break;
        }

        ResetEfOnlyTestState();
    }

    private static async Task<(IHost Host, SystemService SystemService, TaskCompletionSource AboutToStart)> StartServiceUntilAboutToStartAsync(
        bool waitForStartupComplete = false,
        Action<Shoko.Server.Settings.IServerSettings>? configureSettings = null)
    {
        var systemService = new SystemService();
        ApplyCurrentLongRunAppHostNLogClampIfRequested();
        var settings = Utils.SettingsProvider.GetSettings();
        settings.FirstRun = false;
        settings.AniDb.Username = "integration-test";
        settings.AniDb.Password = "integration-test";
        settings.Import.ScanDropFoldersOnStart = false;
        settings.Import.RunOnStart = false;
        settings.Web.Port = GetAvailableTcpPort();
        configureSettings?.Invoke(settings);
        Utils.SettingsProvider.SaveSettings(settings);

        var aboutToStart = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var startupFailed = new TaskCompletionSource<Exception?>(TaskCreationOptions.RunContinuationsAsynchronously);
        systemService.AboutToStart += (_, _) => aboutToStart.TrySetResult();
        systemService.StartupFailed += (_, args) => startupFailed.TrySetResult(args.Exception);

        WriteTestProgress(nameof(StartServiceUntilAboutToStartAsync), "Calling SystemService.StartAsync.");
        var host = await systemService.StartAsync();
        Assert.NotNull(host);

        try
        {
            var completedTask = await Task.WhenAny(
                aboutToStart.Task,
                startupFailed.Task,
                Task.Delay(TimeSpan.FromMinutes(10)));

            if (completedTask == startupFailed.Task)
            {
                throw startupFailed.Task.Result ?? new InvalidOperationException("Startup failed before reaching AboutToStart.");
            }

            if (completedTask != aboutToStart.Task)
            {
                throw new TimeoutException("Timed out waiting for AboutToStart.");
            }

            if (waitForStartupComplete)
            {
                WriteTestProgress(nameof(StartServiceUntilAboutToStartAsync), "Waiting for startup completion inside helper.");
                await systemService.WaitForStartupAsync().WaitAsync(TimeSpan.FromMinutes(10));
                WriteTestProgress(nameof(StartServiceUntilAboutToStartAsync), "Startup completion wait inside helper finished.");
            }
        }
        catch (Exception ex)
        {
            var startupFailure = systemService.StartupFailedException;
            var failureText = startupFailure?.ToString();
            if (startupFailure?.InnerException is not null)
                failureText += Environment.NewLine + startupFailure.InnerException;
            if (startupFailure?.InnerException?.InnerException is not null)
                failureText += Environment.NewLine + startupFailure.InnerException.InnerException;
            throw new InvalidOperationException(failureText ?? ex.ToString(), ex);
        }

        return (host!, systemService, aboutToStart);
    }

    private static async Task WaitForManagedFolderPlaceOrNhTouchAsync(int folderId, string relativePath, TimeSpan timeout)
    {
        using var cancellationTokenSource = new CancellationTokenSource(timeout);
        while (true)
        {
            if (SQLite.SessionFactoryCreateCallCount > 0)
            {
                return;
            }

            using (var scope = Utils.ServiceContainer.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
                var exists = await context.VideoLocal_Place.AsNoTracking()
                    .AnyAsync(place => place.ManagedFolderID == folderId && place.RelativePath == relativePath, cancellationTokenSource.Token);
                if (exists)
                {
                    return;
                }
            }

            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            await Task.Delay(250, cancellationTokenSource.Token);
        }
    }

    private static async Task<(int VideoLocalId, bool ProcessJobScheduled)> WaitForHashedVideoOrNhTouchAsync(int folderId, string relativePath, TimeSpan timeout)
    {
        using var cancellationTokenSource = new CancellationTokenSource(timeout);
        var observedProcessJob = false;
        var startedAt = DateTime.UtcNow;
        var lastProgressAt = startedAt;

        while (true)
        {
            if (SQLite.SessionFactoryCreateCallCount > 0)
            {
                return (0, observedProcessJob);
            }

            using (var scope = Utils.ServiceContainer.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
                var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
                var place = await context.VideoLocal_Place.AsNoTracking()
                    .Where(entry => entry.ManagedFolderID == folderId && entry.RelativePath == relativePath)
                    .Select(entry => new { entry.VideoID })
                    .FirstOrDefaultAsync(cancellationTokenSource.Token);

                if (place is { VideoID: > 0 })
                {
                    var videoLocalId = place.VideoID;
                    var hashReady = await context.VideoLocal.AsNoTracking()
                        .AnyAsync(video => video.VideoLocalID == videoLocalId && !string.IsNullOrEmpty(video.Hash) && video.FileSize > 0, cancellationTokenSource.Token);
                    var digestCount = await context.VideoLocal_HashDigest.AsNoTracking()
                        .CountAsync(digest => digest.VideoLocalID == videoLocalId, cancellationTokenSource.Token);

                    var scheduler = await schedulerFactory.GetScheduler(cancellationTokenSource.Token);
                    var processFileJobKey = JobKeyBuilder<ProcessFileJob>.Create()
                        .WithGroup(JobKeyGroup.Import)
                        .UsingJobData(job => (job.VideoLocalID, job.ForceRecheck, job.ShouldRelocate) = (videoLocalId, true, false))
                        .Build();
                    observedProcessJob |= await scheduler.CheckExists(processFileJobKey, cancellationTokenSource.Token);
                    if (!observedProcessJob)
                    {
                        var executingJobs = await scheduler.GetCurrentlyExecutingJobs(cancellationTokenSource.Token);
                        observedProcessJob = executingJobs.Any(job => job.JobDetail.Key.Equals(processFileJobKey));
                    }

                    if (hashReady && digestCount > 0)
                    {
                        return (videoLocalId, observedProcessJob);
                    }
                }
            }

            WritePollingProgressIfDue(
                ref lastProgressAt,
                startedAt,
                "WaitForHashedVideoOrNhTouchAsync",
                $"Still waiting for hashed video for folderId={folderId}, relativePath='{relativePath}', processJobScheduled={observedProcessJob}.");
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            await Task.Delay(250, cancellationTokenSource.Token);
        }
    }

    private static bool IsHashJobForFile(Shoko.Server.Scheduling.QueueItem item, string relativePath)
    {
        if (!string.Equals(item.JobType, "Hash File", StringComparison.Ordinal))
            return false;

        if (!item.Details.TryGetValue("File Path", out var filePathObj) || filePathObj is not string filePath)
            return false;

        return filePath.Contains(relativePath, StringComparison.Ordinal);
    }

    private static bool IsProcessJobForFile(Shoko.Server.Scheduling.QueueItem item, string relativePath)
    {
        if (!string.Equals(item.JobType, "Get Release Information for Video", StringComparison.Ordinal))
            return false;

        if (!item.Details.TryGetValue("File Path", out var filePathObj) || filePathObj is not string filePath)
            return false;

        return filePath.Contains(relativePath, StringComparison.Ordinal);
    }

    private static bool IsProcessJobForFileOrVideo(Shoko.Server.Scheduling.QueueItem item, string relativePath, int videoLocalId)
    {
        if (IsProcessJobForFile(item, relativePath))
            return true;

        return item.Details.TryGetValue("Video", out var videoObj) && videoObj is int queuedVideoId && queuedVideoId == videoLocalId;
    }

    private static async Task<bool> WaitForHashBoundaryOrObservedAsync(string absolutePath, Func<bool> hashObserved, TimeSpan timeout)
    {
        using var cancellationTokenSource = new CancellationTokenSource(timeout);
        var startedAt = DateTime.UtcNow;
        var lastProgressAt = startedAt;
        while (true)
        {
            if (SQLite.SessionFactoryCreateCallCount > 0)
                return false;

            if (hashObserved())
                return true;

            using (var scope = Utils.ServiceContainer.CreateScope())
            {
                var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
                var scheduler = await schedulerFactory.GetScheduler(cancellationTokenSource.Token);
                var hashFileJobKey = JobKeyBuilder<HashFileJob>.Create()
                    .WithGroup(JobKeyGroup.Import)
                    .UsingJobData(job => job.FilePath = absolutePath)
                    .Build();

                if (await scheduler.CheckExists(hashFileJobKey, cancellationTokenSource.Token))
                    return true;

                var executingJobs = await scheduler.GetCurrentlyExecutingJobs(cancellationTokenSource.Token);
                if (executingJobs.Any(job => job.JobDetail.Key.Equals(hashFileJobKey)))
                    return true;
            }

            WritePollingProgressIfDue(
                ref lastProgressAt,
                startedAt,
                "WaitForHashBoundaryOrObservedAsync",
                $"Still waiting for hash boundary for '{absolutePath}'.");
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            await Task.Delay(250, cancellationTokenSource.Token);
        }
    }

    private static async Task<bool> WaitForProcessJobBoundaryOrObservedAsync(int videoLocalId, string relativePath, Func<bool> processObserved, TimeSpan timeout)
    {
        using var cancellationTokenSource = new CancellationTokenSource(timeout);
        var startedAt = DateTime.UtcNow;
        var lastProgressAt = startedAt;
        while (true)
        {
            if (SQLite.SessionFactoryCreateCallCount > 0)
                return false;

            if (processObserved())
                return true;

            using (var scope = Utils.ServiceContainer.CreateScope())
            {
                var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
                var scheduler = await schedulerFactory.GetScheduler(cancellationTokenSource.Token);
                var processFileJobKey = JobKeyBuilder<ProcessFileJob>.Create()
                    .WithGroup(JobKeyGroup.Import)
                    .UsingJobData(job => (job.VideoLocalID, job.ForceRecheck, job.ShouldRelocate) = (videoLocalId, true, false))
                    .Build();

                if (await scheduler.CheckExists(processFileJobKey, cancellationTokenSource.Token))
                    return true;

                var executingJobs = await scheduler.GetCurrentlyExecutingJobs(cancellationTokenSource.Token);
                if (executingJobs.Any(job => job.JobDetail.Key.Equals(processFileJobKey)))
                    return true;

                if (executingJobs.Any(job => string.Equals(job.JobDetail.Key.Group, JobKeyGroup.Import, StringComparison.Ordinal) &&
                                             job.JobDetail.JobDataMap.TryGetValue(nameof(ProcessFileJob.VideoLocalID), out var queuedVideoIdObj) &&
                                             Convert.ToInt32(queuedVideoIdObj) == videoLocalId))
                    return true;
            }

            WritePollingProgressIfDue(
                ref lastProgressAt,
                startedAt,
                "WaitForProcessJobBoundaryOrObservedAsync",
                $"Still waiting for process boundary for relativePath='{relativePath}', videoLocalId={videoLocalId}.");
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            await Task.Delay(250, cancellationTokenSource.Token);
        }
    }

    private static void WriteTestProgress(string testName, string message)
        => Console.WriteLine($"[{DateTime.UtcNow:O}] [{testName}] {message}");

    private static void WritePollingProgressIfDue(ref DateTime lastProgressAt, DateTime startedAt, string operationName, string message)
    {
        var now = DateTime.UtcNow;
        if (now - lastProgressAt < TimeSpan.FromSeconds(10))
            return;

        lastProgressAt = now;
        Console.WriteLine($"[{now:O}] [{operationName}] +{(now - startedAt):mm\\:ss} {message}");
    }

    private sealed class DisposableAction(Action onDispose) : IDisposable
    {
        private Action? _onDispose = onDispose;

        public void Dispose()
        {
            Interlocked.Exchange(ref _onDispose, null)?.Invoke();
        }
    }

    private static async Task AssertProcessFileJobUsesCachedReleaseOfflineAsync(int videoLocalId)
    {
        var video = RepoFactory.VideoLocal.GetByID(videoLocalId);
        Assert.NotNull(video);
        Assert.False(string.IsNullOrEmpty(video!.Hash));
        Assert.True(video.FileSize > 0);

        if (RepoFactory.StoredReleaseInfo.GetByEd2kAndFileSize(video.Hash, video.FileSize) is null)
        {
            RepoFactory.StoredReleaseInfo.Save(new Shoko.Server.Models.Release.StoredReleaseInfo
            {
                ED2K = video.Hash,
                FileSize = video.FileSize,
                ProviderName = "TestFixture",
                ReleaseURI = $"test://video/{videoLocalId}",
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                EmbeddedCrossReferences = "[]"
            });
        }

        var currentRelease = RepoFactory.StoredReleaseInfo.GetByEd2kAndFileSize(video.Hash, video.FileSize);
        Assert.NotNull(currentRelease);

        var videoReleaseService = Utils.ServiceContainer.GetRequiredService<IVideoReleaseService>();
        var jobFactory = Utils.ServiceContainer.GetRequiredService<JobFactory>();
        var observedSearchStarted = false;
        EventHandler<VideoReleaseSearchStartedEventArgs> onSearchStarted = (_, e) =>
        {
            if (e.Video.ID == videoLocalId)
                observedSearchStarted = true;
        };
        videoReleaseService.SearchStarted += onSearchStarted;

        try
        {
            var processFileJob = jobFactory.CreateJob<ProcessFileJob>(job =>
            {
                job.VideoLocalID = videoLocalId;
                job.ForceRecheck = false;
                job.SkipMyList = true;
                job.ShouldRelocate = false;
            });

            await processFileJob.Process();

            Assert.False(observedSearchStarted, "Expected cached release info to keep ProcessFileJob offline and avoid provider search.");
            Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            videoReleaseService.SearchStarted -= onSearchStarted;
        }
    }

    private static ushort GetAvailableTcpPort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return (ushort)((IPEndPoint)listener.LocalEndpoint).Port;
    }

    private static string GetExpectedSqliteDatabasePath(string shokoHome)
        => Path.Combine(shokoHome, "SQLite", "Shoko.db3");

    private static void CopyExistingSqliteFixtureDatabase(string destinationPath)
    {
        var testProjectDir = Path.GetDirectoryName(typeof(SQLiteEfOnlyBootstrapTests).Assembly.Location)!;
        var solutionDir = Directory.GetParent(testProjectDir)!.Parent!.Parent!.Parent!.FullName;
        var fixturePath = Path.Combine(solutionDir, "spec-backups", "sqlite", "Shoko.db3");
        Assert.True(File.Exists(fixturePath), $"Expected SQLite fixture database at '{fixturePath}'.");

        var destinationDirectory = Path.GetDirectoryName(destinationPath);
        Assert.NotNull(destinationDirectory);
        Directory.CreateDirectory(destinationDirectory);
        File.Copy(fixturePath, destinationPath, overwrite: true);
    }

    private static async Task<bool> EfMigrationsHistoryExistsAsync(string databasePath)
    {
        await using var connection = new SqliteConnection($"Data Source={databasePath}");
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = '__EFMigrationsHistory'";
        return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
    }

    private static async Task<bool> EfMigrationsHistoryContainsMigrationAsync(string databasePath, string migrationId)
    {
        await using var connection = new SqliteConnection($"Data Source={databasePath}");
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM __EFMigrationsHistory WHERE MigrationId = $migrationId";
        command.Parameters.AddWithValue("$migrationId", migrationId);
        return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
    }

    private static Task WriteTinyValidVideoFixtureAsync(string path)
    {
        var bytes = Convert.FromBase64String(string.Concat(
            "AAAAIGZ0eXBpc29tAAACAGlzb21pc28yYXZjMW1wNDEAAARlbW9vdgAAAGxtdmhkAAAAAAAAAAAAAAAAAAAD6AAAA+gAAQAAAQAAAAAAAAAAAAAAAAEAAAAA",
            "AAAAAAAAAAAAAAABAAAAAAAAAAAAAAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAA490cmFrAAAAXHRraGQAAAADAAAAAAAAAAAAAAAB",
            "AAAAAAAAA+gAAAAAAAAAAAAAAAAAAAAAAAEAAAAAAAAAAAAAAAAAAAABAAAAAAAAAAAAAAAAAABAAAAAABAAAAAQAAAAAAAkZWR0cwAAABxlbHN0AAAAAAAA",
            "AAEAAAPoAAAEAAABAAAAAAMHbWRpYQAAACBtZGhkAAAAAAAAAAAAAAAAAAAyAAAAMgBVxAAAAAAALWhkbHIAAAAAAAAAAHZpZGUAAAAAAAAAAAAAAABWaWRl",
            "b0hhbmRsZXIAAAACsm1pbmYAAAAUdm1oZAAAAAEAAAAAAAAAAAAAACRkaW5mAAAAHGRyZWYAAAAAAAAAAQAAAAx1cmwgAAAAAQAAAnJzdGJsAAAAvnN0c2QA",
            "AAAAAAAAAQAAAK5hdmMxAAAAAAAAAAEAAAAAAAAAAAAAAAAAAAAAABAAEABIAAAASAAAAAAAAAABFUxhdmM2Mi4yOC4xMDEgbGlieDI2NAAAAAAAAAAAAAAA",
            "GP//AAAANGF2Y0MBZAAK/+EAF2dkAAqs2V7ARAAAAwAEAAADAMg8SJZYAQAGaOvjyyLA/fj4AAAAABBwYXNwAAAAAQAAAAEAAAAUYnRydAAAAAAAACBoAAAA",
            "AAAAABhzdHRzAAAAAAAAAAEAAAAZAAACAAAAABRzdHNzAAAAAAAAAAEAAAABAAAA2GN0dHMAAAAAAAAAGQAAAAEAAAQAAAAAAQAACgAAAAABAAAEAAAAAAEA",
            "AAAAAAAAAQAAAgAAAAABAAAKAAAAAAEAAAQAAAAAAQAAAAAAAAABAAACAAAAAAEAAAoAAAAAAQAABAAAAAABAAAAAAAAAAEAAAIAAAAAAQAACgAAAAABAAAE",
            "AAAAAAEAAAAAAAAAAQAAAgAAAAABAAAKAAAAAAEAAAQAAAAAAQAAAAAAAAABAAACAAAAAAEAAAoAAAAAAQAABAAAAAABAAAAAAAAAAEAAAIAAAAAHHN0c2MA",
            "AAAAAAAAAQAAAAEAAAAZAAAAAQAAAHhzdHN6AAAAAAAAAAAAAAAZAAACxQAAAAwAAAAMAAAADAAAAAwAAAASAAAADgAAAAwAAAAMAAAAEgAAAA4AAAAMAAAA",
            "DAAAABIAAAAOAAAADAAAAAwAAAASAAAADgAAAAwAAAAMAAAAEgAAAA4AAAAMAAAADAAAABRzdGNvAAAAAAAAAAEAAASVAAAAYnVkdGEAAABabWV0YQAAAAAA",
            "AAAhaGRscgAAAAAAAAAAbWRpcmFwcGwAAAAAAAAAAAAAAAAtaWxzdAAAACWpdG9vAAAAHWRhdGEAAAABAAAAAExhdmY2Mi4xMi4xMDEAAAAIZnJlZQAABBVt",
            "ZGF0AAACrgYF//+q3EXpvebZSLeWLNgg2SPu73gyNjQgLSBjb3JlIDE2NSByMzIyMiBiMzU2MDVhIC0gSC4yNjQvTVBFRy00IEFWQyBjb2RlYyAtIENvcHls",
            "ZWZ0IDIwMDMtMjAyNSAtIGh0dHA6Ly93d3cudmlkZW9sYW4ub3JnL3gyNjQuaHRtbCAtIG9wdGlvbnM6IGNhYmFjPTEgcmVmPTMgZGVibG9jaz0xOjA6MCBh",
            "bmFseXNlPTB4MzoweDExMyBtZT1oZXggc3VibWU9NyBwc3k9MSBwc3lfcmQ9MS4wMDowLjAwIG1peGVkX3JlZj0xIG1lX3JhbmdlPTE2IGNocm9tYV9tZT0x",
            "IHRyZWxsaXM9MSA4eDhkY3Q9MSBjcW09MCBkZWFkem9uZT0yMSwxMSBmYXN0X3Bza2lwPTEgY2hyb21hX3FwX29mZnNldD0tMiB0aHJlYWRzPTEgbG9va2Fo",
            "ZWFkX3RocmVhZHM9MSBzbGljZWRfdGhyZWFkcz0wIG5yPTAgZGVjaW1hdGU9MSBpbnRlcmxhY2VkPTAgYmx1cmF5X2NvbXBhdD0wIGNvbnN0cmFpbmVkX2lu",
            "dHJhPTAgYmZyYW1lcz0zIGJfcHlyYW1pZD0yIGJfYWRhcHQ9MSBiX2JpYXM9MCBkaXJlY3Q9MSB3ZWlnaHRiPTEgb3Blbl9nb3A9MCB3ZWlnaHRwPTIga2V5",
            "aW50PTI1MCBrZXlpbnRfbWluPTI1IHNjZW5lY3V0PTQwIGludHJhX3JlZnJlc2g9MCByY19sb29rYWhlYWQ9NDAgcmM9Y3JmIG1idHJlZT0xIGNyZj0yMy4w",
            "IHFjb21wPTAuNjAgcXBtaW49MCBxcG1heD02OSBxcHN0ZXA9NCBpcF9yYXRpbz0xLjQwIGFxPTE6MS4wMACAAAAAD2WIhAA7//73Tr8Cm1TCYQAAAAhBmiRs",
            "Q7/+4AAAAAhBnkJ4hf/BgQAAAAgBnmF0Qr/EgAAAAAgBnmNqQr/EgQAAAA5BmmhJqEFomUwId//+4QAAAApBnoZFESwv/8GBAAAACAGepXRCv8SBAAAACAGe",
            "p2pCv8SAAAAADkGarEmoQWyZTAh3//7gAAAACkGeykUVLC//wYEAAAAIAZ7pdEK/xIAAAAAIAZ7rakK/xIAAAAAOQZrwSahBbJlMCG///uEAAAAKQZ8ORRUs",
            "L//BgQAAAAgBny10Qr/EgQAAAAgBny9qQr/EgAAAAA5BmzRJqEFsmUwIZ//+4AAAAApBn1JFFSwv/8GBAAAACAGfcXRCv8SAAAAACAGfc2pCv8SAAAAADkGb",
            "eEmoQWyZTAhX//7BAAAACkGflkUVLC//wYAAAAAIAZ+1dEK/xIEAAAAIAZ+3akK/xIE="));
        return File.WriteAllBytesAsync(path, bytes);
    }

    private static async Task DeleteDirectoryWithRetriesAsync(string path)
    {
        if (!Directory.Exists(path))
            return;

        for (var attempt = 0; attempt < 10; attempt++)
        {
            try
            {
                Directory.Delete(path, recursive: true);
                return;
            }
            catch (IOException) when (attempt < 9)
            {
                await Task.Delay(250);
            }
            catch (UnauthorizedAccessException) when (attempt < 9)
            {
                await Task.Delay(250);
            }
        }
    }
}
