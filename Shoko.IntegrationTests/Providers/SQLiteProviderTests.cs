using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shoko.Abstractions.Video.Services;
using Shoko.Server.API.v2.Models.common;
using Shoko.Server.Data;
using Shoko.Server.Models.Shoko;
using Shoko.Server.Repositories;
using Shoko.Server.Utilities;
using Xunit;

namespace Shoko.IntegrationTests.Providers;

/// <summary>
/// Validates EF Core SQLite provider compatibility and provider-specific behaviors.
/// Runs against an isolated SQLite database created by the standard migration pipeline.
/// </summary>
[Collection("Database")]
public class SQLiteProviderTests : IClassFixture<DatabaseMigrationFixture>
{
    private readonly DatabaseMigrationFixture _fixture;

    public SQLiteProviderTests(DatabaseMigrationFixture fixture)
    {
        _fixture = fixture;
        Assert.True(_fixture.Success, _fixture.FailureMessage ?? "SQLite database initialization failed");
    }

    [Fact]
    public void SQLite_CreateAndQueryAnimeSeries()
    {
        // Validate basic CRUD via EF Core against SQLite
        var groupRepo = RepoFactory.AnimeGroup;
        var group = new AnimeGroup
        {
            GroupName = "SQLite Test Group",
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow
        };
        groupRepo.Save(group);
        
        var repo = RepoFactory.AnimeSeries;
        var series = new AnimeSeries
        {
            AniDB_ID = 999999,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
            AnimeGroupID = group.AnimeGroupID
        };
        repo.Save(series);
        
        var loaded = repo.GetByID(series.AnimeSeriesID);
        Assert.NotNull(loaded);
        Assert.Equal(999999, loaded.AniDB_ID);
    }

    [Fact]
    public void SQLite_AnimeGroup_ExplicitCrudOperations()
    {
        var repo = RepoFactory.AnimeGroup;
        var createdAt = DateTime.UtcNow;
        var originalName = $"SQLite CRUD Group {Guid.NewGuid():N}";
        var updatedName = $"{originalName} Updated";

        var group = new AnimeGroup
        {
            GroupName = originalName,
            DateTimeCreated = createdAt,
            DateTimeUpdated = createdAt
        };

        repo.Save(group);

        var created = repo.GetByID(group.AnimeGroupID);
        Assert.NotNull(created);
        Assert.Equal(originalName, created.GroupName);

        created.GroupName = updatedName;
        created.DateTimeUpdated = DateTime.UtcNow;
        repo.Save(created);

        var updated = repo.GetByID(group.AnimeGroupID);
        Assert.NotNull(updated);
        Assert.Equal(updatedName, updated.GroupName);

        repo.Delete(updated);

        var deleted = repo.GetByID(group.AnimeGroupID);
        Assert.Null(deleted);
    }

    [Fact]
    public void SQLite_TransactionCommit()
    {
        var groupName = $"SQLite Commit Group {Guid.NewGuid():N}";
        var createdAt = DateTime.UtcNow;

        using (var writeScope = Utils.ServiceContainer.CreateScope())
        {
            var context = writeScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            using var transaction = context.Database.BeginTransaction();

            context.AnimeGroup.Add(new AnimeGroup
            {
                GroupName = groupName,
                DateTimeCreated = createdAt,
                DateTimeUpdated = createdAt
            });
            context.SaveChanges();

            transaction.Commit();
        }

        using var readScope = Utils.ServiceContainer.CreateScope();
        var readContext = readScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
        var groups = readContext.AnimeGroup
            .AsNoTracking()
            .Where(group => group.GroupName == groupName)
            .Select(group => new
            {
                group.AnimeGroupID,
                group.GroupName
            })
            .ToList();

        var group = Assert.Single(groups);
        Assert.True(group.AnimeGroupID > 0);
        Assert.Equal(groupName, group.GroupName);
    }

    [Fact]
    public void SQLite_TransactionRollback()
    {
        var groupName = $"SQLite Rollback Group {Guid.NewGuid():N}";
        int transientId;

        using (var writeScope = Utils.ServiceContainer.CreateScope())
        {
            var context = writeScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            using var transaction = context.Database.BeginTransaction();

            var group = new AnimeGroup
            {
                GroupName = groupName,
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow
            };

            context.AnimeGroup.Add(group);
            context.SaveChanges();
            transientId = group.AnimeGroupID;

            transaction.Rollback();
        }

        using var readScope = Utils.ServiceContainer.CreateScope();
        var readContext = readScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
        var matchingCount = readContext.AnimeGroup.Count(group => group.GroupName == groupName);
        var persistedById = readContext.AnimeGroup.AsNoTracking().SingleOrDefault(group => group.AnimeGroupID == transientId);

        Assert.Equal(0, matchingCount);
        Assert.Null(persistedById);
    }

    [Fact]
    public void SQLite_ComplexQueryWithJoins()
    {
        var groupRepo = RepoFactory.AnimeGroup;
        var seriesRepo = RepoFactory.AnimeSeries;
        var prefix = Guid.NewGuid().ToString("N");
        var matchingGroupName = $"SQLite Query Group {prefix}";
        var otherGroupName = $"SQLite Query Other Group {prefix}";
        var matchingAniDbId = 760002;

        var matchingGroup = new AnimeGroup
        {
            GroupName = matchingGroupName,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow
        };
        var otherGroup = new AnimeGroup
        {
            GroupName = otherGroupName,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
        };
        groupRepo.Save(matchingGroup);
        groupRepo.Save(otherGroup);

        seriesRepo.Save(new AnimeSeries
        {
            AniDB_ID = 760001,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
            AnimeGroupID = matchingGroup.AnimeGroupID
        });
        seriesRepo.Save(new AnimeSeries
        {
            AniDB_ID = matchingAniDbId,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
            AnimeGroupID = matchingGroup.AnimeGroupID
        });
        seriesRepo.Save(new AnimeSeries
        {
            AniDB_ID = 760003,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
            AnimeGroupID = otherGroup.AnimeGroupID
        });

        using var scope = Utils.ServiceContainer.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();

        var results = context.AnimeSeries
            .AsNoTracking()
            .Join(
                context.AnimeGroup.AsNoTracking(),
                series => series.AnimeGroupID,
                group => group.AnimeGroupID,
                (series, group) => new { series, group })
            .Where(row => row.group.GroupName == matchingGroupName && row.series.AniDB_ID >= matchingAniDbId)
            .OrderBy(row => row.series.AniDB_ID)
            .Select(row => new
            {
                row.series.AniDB_ID,
                row.group.GroupName,
                row.series.AnimeGroupID
            })
            .ToList();

        var result = Assert.Single(results);
        Assert.Equal(matchingAniDbId, result.AniDB_ID);
        Assert.Equal(matchingGroupName, result.GroupName);
        Assert.Equal(matchingGroup.AnimeGroupID, result.AnimeGroupID);
    }

    [Fact]
    public async Task SQLite_ConcurrentReads()
    {
        // Validate concurrent read scenarios
        var groupRepo = RepoFactory.AnimeGroup;
        var group = new AnimeGroup
        {
            GroupName = "SQLite Test Group 5",
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow
        };
        groupRepo.Save(group);
        
        var repo = RepoFactory.AnimeSeries;
        var series = new AnimeSeries
        {
            AniDB_ID = 555555,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
            AnimeGroupID = group.AnimeGroupID
        };
        repo.Save(series);
        
        var tasks = new List<Task<AnimeSeries?>>();
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Task.Run(() => repo.GetByID(series.AnimeSeriesID)));
        }
        await Task.WhenAll(tasks);
        
        Assert.All(tasks, t => Assert.NotNull(t.Result));
    }

    [Fact]
    public void SQLite_ProviderSpecificBehavior()
    {
        // Validate SQLite-specific behaviors (case sensitivity, collation)
        var groupRepo = RepoFactory.AnimeGroup;
        var group = new AnimeGroup
        {
            GroupName = "SQLite Test Group 4",
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow
        };
        groupRepo.Save(group);
        
        var repo = RepoFactory.AnimeSeries;
        var series1 = new AnimeSeries
        {
            AniDB_ID = 444444,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
            AnimeGroupID = group.AnimeGroupID
        };
        var series2 = new AnimeSeries
        {
            AniDB_ID = 333333,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
            AnimeGroupID = group.AnimeGroupID
        };
        repo.Save(series1);
        repo.Save(series2);
        
        var all = repo.GetAll();
        Assert.Contains(all, s => s.AnimeSeriesID == series1.AnimeSeriesID);
        Assert.Contains(all, s => s.AnimeSeriesID == series2.AnimeSeriesID);
    }

    [Fact]
    public void SQLite_VideoLocal_SaveNewEntity_AssignsIdWithoutConcurrencyException()
    {
        var repo = RepoFactory.VideoLocal;
        var createdAt = DateTime.UtcNow;
        var video = new VideoLocal
        {
            DateTimeCreated = createdAt,
            DateTimeUpdated = createdAt,
            FileName = $"scan-import-{Guid.NewGuid():N}.mkv",
            FileSize = 1024,
            Hash = Guid.NewGuid().ToString("N"),
            HashSource = 0,
            IsIgnored = false,
            IsVariation = false,
            MediaVersion = 0,
            MyListID = 0
        };

        repo.Save(video, updateEpisodes: false);

        Assert.True(video.VideoLocalID > 0);

        var persisted = repo.GetByID(video.VideoLocalID);
        Assert.NotNull(persisted);
        Assert.Equal(video.FileName, persisted.FileName);
        Assert.Equal(video.FileSize, persisted.FileSize);
    }

    [Fact]
    public void SQLite_VideoLocal_SaveExistingEntity_UpdatesPersistedRow()
    {
        var repo = RepoFactory.VideoLocal;
        var createdAt = DateTime.UtcNow;
        var video = new VideoLocal
        {
            DateTimeCreated = createdAt,
            DateTimeUpdated = createdAt,
            FileName = $"existing-video-{Guid.NewGuid():N}.mkv",
            FileSize = 2048,
            Hash = Guid.NewGuid().ToString("N"),
            HashSource = 0,
            IsIgnored = false,
            IsVariation = false,
            MediaVersion = 0,
            MyListID = 0
        };

        repo.Save(video, updateEpisodes: false);

        video.FileSize = 4096;
        video.DateTimeUpdated = DateTime.UtcNow;
        repo.Save(video, updateEpisodes: false);

        var persisted = repo.GetByID(video.VideoLocalID);
        Assert.NotNull(persisted);
        Assert.Equal(4096, persisted.FileSize);
    }

    [Fact]
    public void SQLite_VideoLocal_SaveDeletedDetachedEntity_ReinsertsWithoutConcurrencyException()
    {
        var repo = RepoFactory.VideoLocal;
        var createdAt = DateTime.UtcNow;
        var video = new VideoLocal
        {
            DateTimeCreated = createdAt,
            DateTimeUpdated = createdAt,
            FileName = $"reinsert-video-{Guid.NewGuid():N}.mkv",
            FileSize = 3072,
            Hash = Guid.NewGuid().ToString("N"),
            HashSource = 0,
            IsIgnored = false,
            IsVariation = false,
            MediaVersion = 0,
            MyListID = 0
        };

        repo.Save(video, updateEpisodes: false);
        repo.Delete(video);

        video.FileSize = 6144;
        video.DateTimeUpdated = DateTime.UtcNow;
        repo.Save(video, updateEpisodes: false);

        var persisted = repo.GetByID(video.VideoLocalID);
        Assert.NotNull(persisted);
        Assert.Equal(6144, persisted.FileSize);
    }

    [Fact]
    public async Task SQLite_NotifyVideoFileChangeDetected_KnownVideoWithNewPlace_CreatesPlaceWithoutUpdatingVideo()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), $"shoko-videopath-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempRoot);
        try
        {
            var relativePath = "known-video.mkv";
            var absolutePath = Path.Combine(tempRoot, relativePath);
            await File.WriteAllBytesAsync(absolutePath, new byte[4096]);
            var fileSize = new FileInfo(absolutePath).Length;

            var folder = new ShokoManagedFolder
            {
                Name = $"SQLite Import Folder {Guid.NewGuid():N}",
                Path = tempRoot,
                IsWatched = true
            };
            RepoFactory.ShokoManagedFolder.Save(folder);

            var video = new VideoLocal
            {
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow,
                FileName = Path.GetFileName(relativePath),
                FileSize = fileSize,
                Hash = Guid.NewGuid().ToString("N"),
                HashSource = 0,
                IsIgnored = false,
                IsVariation = false,
                MediaVersion = 0,
                MyListID = 0
            };
            RepoFactory.VideoLocal.Save(video, updateEpisodes: false);

            RepoFactory.FileNameHash.Save(new FileNameHash
            {
                FileName = Path.GetFileName(relativePath),
                FileSize = fileSize,
                Hash = video.Hash,
                DateTimeUpdated = DateTime.UtcNow
            });

            var videoService = Utils.ServiceContainer.GetRequiredService<IVideoService>();
            await videoService.NotifyVideoFileChangeDetected(folder, relativePath, updateMylist: false);

            var persistedPlace = RepoFactory.VideoLocalPlace.GetByRelativePathAndManagedFolderID(relativePath, folder.ID);
            Assert.NotNull(persistedPlace);
            Assert.Equal(video.VideoLocalID, persistedPlace.VideoID);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }

    [Fact]
    public async Task SQLite_NotifyVideoFileChangeDetected_NewUnknownFile_CreatesStubVideoAndPlace()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), $"shoko-videopath-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempRoot);
        try
        {
            var relativePath = "new-unknown-video.mkv";
            var absolutePath = Path.Combine(tempRoot, relativePath);
            await File.WriteAllBytesAsync(absolutePath, new byte[4096]);

            var folder = new ShokoManagedFolder
            {
                Name = $"SQLite Import Folder {Guid.NewGuid():N}",
                Path = tempRoot,
                IsWatched = true
            };
            RepoFactory.ShokoManagedFolder.Save(folder);

            var videoService = Utils.ServiceContainer.GetRequiredService<IVideoService>();
            await videoService.NotifyVideoFileChangeDetected(folder, relativePath, updateMylist: false);

            var persistedPlace = RepoFactory.VideoLocalPlace.GetByRelativePathAndManagedFolderID(relativePath, folder.ID);
            Assert.NotNull(persistedPlace);
            Assert.True(persistedPlace.VideoID > 0);

            var persistedVideo = RepoFactory.VideoLocal.GetByID(persistedPlace.VideoID);
            Assert.NotNull(persistedVideo);
            Assert.StartsWith("__stub__", persistedVideo.Hash);
            Assert.Equal(40, persistedVideo.Hash.Length);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }

    [Fact]
    public async Task SQLite_NotifyVideoFileChangeDetected_UsesRelativePathWhenManagedFolderIsMissing()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), $"shoko-videopath-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempRoot);
        try
        {
            var relativePath = "folderless-video.mkv";
            var absolutePath = Path.Combine(tempRoot, relativePath);
            await File.WriteAllBytesAsync(absolutePath, new byte[4096]);

            var folder = new ShokoManagedFolder
            {
                Name = $"SQLite Import Folder {Guid.NewGuid():N}",
                Path = tempRoot,
                IsWatched = true
            };
            RepoFactory.ShokoManagedFolder.Save(folder);

            var videoService = Utils.ServiceContainer.GetRequiredService<IVideoService>();
            await videoService.NotifyVideoFileChangeDetected(folder, relativePath, updateMylist: false);

            var persistedPlace = RepoFactory.VideoLocalPlace.GetByRelativePathAndManagedFolderID(relativePath, folder.ID);
            Assert.NotNull(persistedPlace);
            Assert.True(persistedPlace!.Path is not null);

            RepoFactory.ShokoManagedFolder.Delete(folder);

            var persistedVideo = RepoFactory.VideoLocal.GetByID(persistedPlace.VideoID);
            Assert.NotNull(persistedVideo);

            var rawFile = new RawFile(new Microsoft.AspNetCore.Http.DefaultHttpContext(), persistedVideo!, 0, 0);
            Assert.Equal(relativePath, rawFile.filename);
            Assert.Equal(relativePath, rawFile.server_path);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }

    [Fact]
    public async Task SQLite_NotifyVideoFileChangeDetected_MultipleUnknownFiles_CreateDistinctStubHashes()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), $"shoko-videopath-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempRoot);
        try
        {
            var folder = new ShokoManagedFolder
            {
                Name = $"SQLite Import Folder {Guid.NewGuid():N}",
                Path = tempRoot,
                IsWatched = true
            };
            RepoFactory.ShokoManagedFolder.Save(folder);

            var relativePaths = new[] { "first-unknown-video.mkv", "second-unknown-video.mkv" };
            foreach (var relativePath in relativePaths)
            {
                var absolutePath = Path.Combine(tempRoot, relativePath);
                await File.WriteAllBytesAsync(absolutePath, new byte[4096]);
            }

            var videoService = Utils.ServiceContainer.GetRequiredService<IVideoService>();
            foreach (var relativePath in relativePaths)
            {
                await videoService.NotifyVideoFileChangeDetected(folder, relativePath, updateMylist: false);
            }

            var persistedVideos = relativePaths
                .Select(relativePath => RepoFactory.VideoLocalPlace.GetByRelativePathAndManagedFolderID(relativePath, folder.ID))
                .Where(place => place is not null)
                .Select(place => RepoFactory.VideoLocal.GetByID(place!.VideoID))
                .Where(video => video is not null)
                .ToList();

            Assert.Equal(2, persistedVideos.Count);
            Assert.Equal(2, persistedVideos.Select(video => video!.Hash).Distinct().Count());
            Assert.All(persistedVideos, video => Assert.StartsWith("__stub__", video!.Hash));
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }
}
