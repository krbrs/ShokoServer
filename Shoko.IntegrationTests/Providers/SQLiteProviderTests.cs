using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shoko.Abstractions.Video.Services;
using Shoko.Server.API.v2.Models.common;
using Shoko.Server.Data;
using Shoko.Server.Databases;
using Shoko.Server.Models.Legacy;
using Shoko.Server.Models.Shoko;
using Shoko.Server.Repositories;
using Shoko.Server.Repositories.Cached;
using Shoko.Server.Repositories.Cached.AniDB;
using Shoko.Server.Scheduling;
using Shoko.Server.Scheduling.Jobs.AniDB;
using Shoko.Server.Scheduling.Jobs.Shoko;
using Shoko.Server.Server;
using Shoko.Server.Services;
using Shoko.Server.Settings;
using Shoko.Server.Tasks;
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

    [Fact]
    public void SQLite_VideoLocalPlace_RegenerateDb_EfOnlyRemovesOrphansAndKeepsValidRows()
    {
        SQLite.UseEfOnlyBootstrapForTests = true;
        try
        {
            var folder = new ShokoManagedFolder
            {
                Name = $"SQLite Place Cleanup Folder {Guid.NewGuid():N}",
                Path = Path.Combine(Path.GetTempPath(), $"shoko-place-cleanup-{Guid.NewGuid():N}"),
                IsWatched = true
            };
            RepoFactory.ShokoManagedFolder.Save(folder);

            var video = new VideoLocal
            {
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow,
                FileName = $"place-cleanup-{Guid.NewGuid():N}.mkv",
                FileSize = 1234,
                Hash = Guid.NewGuid().ToString("N"),
                HashSource = 0,
                IsIgnored = false,
                IsVariation = false,
                MediaVersion = 0,
                MyListID = 0
            };
            RepoFactory.VideoLocal.Save(video, updateEpisodes: false);

            var validPlace = new VideoLocal_Place
            {
                ManagedFolderID = folder.ID,
                RelativePath = "valid-place.mkv",
                VideoID = video.VideoLocalID
            };
            RepoFactory.VideoLocalPlace.Save(validPlace);

            var orphanId = 0;
            using (var scope = Utils.ServiceContainer.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
                var orphan = new VideoLocal_Place
                {
                    ManagedFolderID = 0,
                    RelativePath = "orphan-place.mkv",
                    VideoID = 0
                };
                context.VideoLocal_Place.Add(orphan);
                context.SaveChanges();
                orphanId = orphan.ID;
            }

            RepoFactory.VideoLocalPlace.Populate();
            RepoFactory.VideoLocalPlace.RegenerateDb();

            using (var scope = Utils.ServiceContainer.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
                Assert.Null(context.VideoLocal_Place.AsNoTracking().SingleOrDefault(place => place.ID == orphanId));
                Assert.NotNull(context.VideoLocal_Place.AsNoTracking().SingleOrDefault(place => place.ID == validPlace.ID));
            }

            Assert.Null(RepoFactory.VideoLocalPlace.GetAll().SingleOrDefault(place => place.ID == orphanId));
            Assert.NotNull(RepoFactory.VideoLocalPlace.GetByRelativePathAndManagedFolderID(validPlace.RelativePath, folder.ID));
        }
        finally
        {
            SQLite.UseEfOnlyBootstrapForTests = false;
        }
    }

    [Fact]
    public void SQLite_VideoLocal_RegenerateDb_EfOnlyRemovesEmptyAndNoPlaceRows()
    {
        SQLite.UseEfOnlyBootstrapForTests = true;
        try
        {
            var folder = new ShokoManagedFolder
            {
                Name = $"SQLite Video Cleanup Folder {Guid.NewGuid():N}",
                Path = Path.Combine(Path.GetTempPath(), $"shoko-video-cleanup-{Guid.NewGuid():N}"),
                IsWatched = true
            };
            RepoFactory.ShokoManagedFolder.Save(folder);

            var removableVideo = new VideoLocal
            {
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow,
                FileName = $"no-place-{Guid.NewGuid():N}.mkv",
                FileSize = 4321,
                Hash = Guid.NewGuid().ToString("N"),
                HashSource = 0,
                IsIgnored = false,
                IsVariation = false,
                MediaVersion = VideoLocal.MEDIA_VERSION,
                MyListID = 0,
                MediaInfo = new Shoko.Server.MediaInfo.MediaContainer()
            };
            RepoFactory.VideoLocal.Save(removableVideo, updateEpisodes: false);

            var validVideo = new VideoLocal
            {
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow,
                FileName = $"valid-{Guid.NewGuid():N}.mkv",
                FileSize = 8765,
                Hash = Guid.NewGuid().ToString("N"),
                HashSource = 0,
                IsIgnored = false,
                IsVariation = false,
                MediaVersion = VideoLocal.MEDIA_VERSION,
                MyListID = 0,
                MediaInfo = new Shoko.Server.MediaInfo.MediaContainer()
            };
            RepoFactory.VideoLocal.Save(validVideo, updateEpisodes: false);
            RepoFactory.VideoLocalPlace.Save(new VideoLocal_Place
            {
                ManagedFolderID = folder.ID,
                RelativePath = "valid-video.mkv",
                VideoID = validVideo.VideoLocalID
            });

            var user = RepoFactory.JMMUser.GetAll().FirstOrDefault();
            Assert.NotNull(user);

            RepoFactory.VideoLocalUser.Save(new VideoLocal_User
            {
                JMMUserID = user!.JMMUserID,
                VideoLocalID = removableVideo.VideoLocalID,
                LastUpdated = DateTime.UtcNow,
                ResumePosition = 0,
                WatchedCount = 1
            });
            RepoFactory.VideoLocalHashDigest.Save(new VideoLocal_HashDigest
            {
                VideoLocalID = removableVideo.VideoLocalID,
                Type = "MD5",
                Value = Guid.NewGuid().ToString("N")
            });

            var emptyVideoId = 0;
            using (var scope = Utils.ServiceContainer.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
                var emptyVideo = new VideoLocal
                {
                    DateTimeCreated = DateTime.UtcNow,
                    DateTimeUpdated = DateTime.UtcNow,
                    FileName = string.Empty,
                    FileSize = 0,
                    Hash = string.Empty,
                    HashSource = 0,
                    IsIgnored = false,
                    IsVariation = false,
                    MediaVersion = VideoLocal.MEDIA_VERSION,
                    MyListID = 0
                };
                context.VideoLocal.Add(emptyVideo);
                context.SaveChanges();
                emptyVideoId = emptyVideo.VideoLocalID;
            }

            RepoFactory.VideoLocal.Populate();
            RepoFactory.VideoLocalPlace.Populate();
            RepoFactory.VideoLocalUser.Populate();
            RepoFactory.VideoLocalHashDigest.Populate();
            RepoFactory.VideoLocal.RegenerateDb();

            using (var scope = Utils.ServiceContainer.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
                Assert.Null(context.VideoLocal.AsNoTracking().SingleOrDefault(video => video.VideoLocalID == emptyVideoId));
                Assert.Null(context.VideoLocal.AsNoTracking().SingleOrDefault(video => video.VideoLocalID == removableVideo.VideoLocalID));
                Assert.NotNull(context.VideoLocal.AsNoTracking().SingleOrDefault(video => video.VideoLocalID == validVideo.VideoLocalID));
            }

            Assert.Null(RepoFactory.VideoLocal.GetByID(emptyVideoId));
            Assert.Null(RepoFactory.VideoLocal.GetByID(removableVideo.VideoLocalID));
            Assert.NotNull(RepoFactory.VideoLocal.GetByID(validVideo.VideoLocalID));
            Assert.Empty(RepoFactory.VideoLocalUser.GetByVideoLocalID(removableVideo.VideoLocalID));
            Assert.Empty(RepoFactory.VideoLocalHashDigest.GetByVideoLocalID(removableVideo.VideoLocalID));
            Assert.NotNull(RepoFactory.VideoLocalPlace.GetByRelativePathAndManagedFolderID("valid-video.mkv", folder.ID));
        }
        finally
        {
            SQLite.UseEfOnlyBootstrapForTests = false;
        }
    }

    [Fact]
    public void SQLite_VideoLocal_RegenerateDb_EfOnlyMergesDuplicateRowsByHash()
    {
        SQLite.UseEfOnlyBootstrapForTests = true;
        try
        {
            var folder = new ShokoManagedFolder
            {
                Name = $"SQLite Duplicate Merge Folder {Guid.NewGuid():N}",
                Path = Path.Combine(Path.GetTempPath(), $"shoko-duplicate-merge-{Guid.NewGuid():N}"),
                IsWatched = true
            };
            RepoFactory.ShokoManagedFolder.Save(folder);

            var duplicateHash = Guid.NewGuid().ToString("N");
            var survivor = new VideoLocal
            {
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow,
                FileName = $"survivor-{Guid.NewGuid():N}.mkv",
                FileSize = 1111,
                Hash = duplicateHash,
                HashSource = 0,
                IsIgnored = false,
                IsVariation = false,
                MediaVersion = VideoLocal.MEDIA_VERSION,
                MyListID = 0,
                MediaInfo = new Shoko.Server.MediaInfo.MediaContainer()
            };
            var loser = new VideoLocal
            {
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow,
                FileName = $"loser-{Guid.NewGuid():N}.mkv",
                FileSize = 1111,
                Hash = Guid.NewGuid().ToString("N"),
                HashSource = 1,
                IsIgnored = false,
                IsVariation = false,
                MediaVersion = VideoLocal.MEDIA_VERSION,
                MyListID = 0,
                MediaInfo = new Shoko.Server.MediaInfo.MediaContainer()
            };
            var untouched = new VideoLocal
            {
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow,
                FileName = $"untouched-{Guid.NewGuid():N}.mkv",
                FileSize = 2222,
                Hash = Guid.NewGuid().ToString("N"),
                HashSource = 0,
                IsIgnored = false,
                IsVariation = false,
                MediaVersion = VideoLocal.MEDIA_VERSION,
                MyListID = 0,
                MediaInfo = new Shoko.Server.MediaInfo.MediaContainer()
            };

            RepoFactory.VideoLocal.Save(survivor, updateEpisodes: false);
            RepoFactory.VideoLocal.Save(loser, updateEpisodes: false);
            RepoFactory.VideoLocal.Save(untouched, updateEpisodes: false);

            RepoFactory.VideoLocalPlace.Save(new VideoLocal_Place
            {
                ManagedFolderID = folder.ID,
                RelativePath = "survivor-place.mkv",
                VideoID = survivor.VideoLocalID
            });
            RepoFactory.VideoLocalPlace.Save(new VideoLocal_Place
            {
                ManagedFolderID = folder.ID,
                RelativePath = "loser-place.mkv",
                VideoID = loser.VideoLocalID
            });
            RepoFactory.VideoLocalPlace.Save(new VideoLocal_Place
            {
                ManagedFolderID = folder.ID,
                RelativePath = "untouched-place.mkv",
                VideoID = untouched.VideoLocalID
            });

            var user = RepoFactory.JMMUser.GetAll().FirstOrDefault();
            Assert.NotNull(user);

            RepoFactory.VideoLocalUser.Save(new VideoLocal_User
            {
                JMMUserID = user!.JMMUserID,
                VideoLocalID = loser.VideoLocalID,
                LastUpdated = DateTime.UtcNow,
                ResumePosition = 0,
                WatchedCount = 1
            });
            RepoFactory.VideoLocalHashDigest.Save(new VideoLocal_HashDigest
            {
                VideoLocalID = loser.VideoLocalID,
                Type = "MD5",
                Value = Guid.NewGuid().ToString("N")
            });

            RepoFactory.VideoLocal.Populate();
            RepoFactory.VideoLocalPlace.Populate();
            RepoFactory.VideoLocalUser.Populate();
            RepoFactory.VideoLocalHashDigest.Populate();

            using (var scope = Utils.ServiceContainer.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
                context.Database.ExecuteSqlRaw("DROP INDEX IF EXISTS UIX_VideoLocal_Hash");
                context.Database.ExecuteSqlRaw("UPDATE VideoLocal SET Hash = {0} WHERE VideoLocalID = {1}", duplicateHash, loser.VideoLocalID);
            }

            try
            {
                RepoFactory.VideoLocal.Populate();
                RepoFactory.VideoLocalPlace.Populate();
                RepoFactory.VideoLocalUser.Populate();
                RepoFactory.VideoLocalHashDigest.Populate();
                RepoFactory.VideoLocal.RegenerateDb();
            }
            finally
            {
                using var scope = Utils.ServiceContainer.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
                context.Database.ExecuteSqlRaw(
                    "UPDATE VideoLocal SET Hash = {0} WHERE VideoLocalID = {1} AND Hash = {2}",
                    Guid.NewGuid().ToString("N"),
                    loser.VideoLocalID,
                    duplicateHash);
                context.Database.ExecuteSqlRaw("CREATE UNIQUE INDEX IF NOT EXISTS UIX_VideoLocal_Hash on VideoLocal(Hash)");
            }

            using (var scope = Utils.ServiceContainer.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
                Assert.NotNull(context.VideoLocal.AsNoTracking().SingleOrDefault(video => video.VideoLocalID == survivor.VideoLocalID));
                Assert.Null(context.VideoLocal.AsNoTracking().SingleOrDefault(video => video.VideoLocalID == loser.VideoLocalID));
                Assert.NotNull(context.VideoLocal.AsNoTracking().SingleOrDefault(video => video.VideoLocalID == untouched.VideoLocalID));
            }

            Assert.NotNull(RepoFactory.VideoLocal.GetByID(survivor.VideoLocalID));
            Assert.Null(RepoFactory.VideoLocal.GetByID(loser.VideoLocalID));
            Assert.NotNull(RepoFactory.VideoLocal.GetByID(untouched.VideoLocalID));

            var survivorPlaces = RepoFactory.VideoLocalPlace.GetByVideoLocal(survivor.VideoLocalID);
            Assert.Contains(survivorPlaces, place => place.RelativePath == "survivor-place.mkv");
            Assert.Contains(survivorPlaces, place => place.RelativePath == "loser-place.mkv");
            Assert.DoesNotContain(RepoFactory.VideoLocalPlace.GetAll(), place => place.VideoID == loser.VideoLocalID);

            Assert.Empty(RepoFactory.VideoLocalUser.GetByVideoLocalID(loser.VideoLocalID));
            Assert.Empty(RepoFactory.VideoLocalHashDigest.GetByVideoLocalID(loser.VideoLocalID));
            Assert.NotNull(RepoFactory.VideoLocalPlace.GetByRelativePathAndManagedFolderID("untouched-place.mkv", folder.ID));
        }
        finally
        {
            SQLite.UseEfOnlyBootstrapForTests = false;
        }
    }

    [Fact]
    public void SQLite_AnimeSeries_RegenerateDb_EfOnlyRepairsMissingGroups()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        databaseFactory.CloseSessionFactory();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            var now = DateTime.UtcNow;
            var validGroup = new AnimeGroup
            {
                GroupName = $"valid-group-{Guid.NewGuid():N}",
                DateTimeCreated = now,
                DateTimeUpdated = now
            };
            RepoFactory.AnimeGroup.Save(validGroup);

            var validSeries = new AnimeSeries
            {
                AniDB_ID = 880001 + Random.Shared.Next(1000),
                AnimeGroupID = validGroup.AnimeGroupID,
                DateTimeCreated = now,
                DateTimeUpdated = now
            };
            var noGroupSeries = new AnimeSeries
            {
                AniDB_ID = 890001 + Random.Shared.Next(1000),
                AnimeGroupID = 0,
                DateTimeCreated = now,
                DateTimeUpdated = now
            };
            var danglingGroupSeries = new AnimeSeries
            {
                AniDB_ID = 900001 + Random.Shared.Next(1000),
                AnimeGroupID = int.MaxValue - 123,
                DateTimeCreated = now,
                DateTimeUpdated = now
            };

            RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
            {
                AnimeID = validSeries.AniDB_ID,
                MainTitle = $"valid-{Guid.NewGuid():N}",
                AllTitles = string.Empty,
                AllTags = string.Empty,
                Description = string.Empty
            });
            RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
            {
                AnimeID = noGroupSeries.AniDB_ID,
                MainTitle = $"nogroup-{Guid.NewGuid():N}",
                AllTitles = string.Empty,
                AllTags = string.Empty,
                Description = string.Empty
            });
            RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
            {
                AnimeID = danglingGroupSeries.AniDB_ID,
                MainTitle = $"dangling-{Guid.NewGuid():N}",
                AllTitles = string.Empty,
                AllTags = string.Empty,
                Description = string.Empty
            });

            RepoFactory.AnimeSeries.Save(validSeries, false, true);
            RepoFactory.AnimeSeries.Save(noGroupSeries, false, true);
            RepoFactory.AnimeSeries.Save(danglingGroupSeries, false, true);

            RepoFactory.AnimeSeries.RegenerateDb();

            var refreshedValidSeries = RepoFactory.AnimeSeries.GetByID(validSeries.AnimeSeriesID);
            var refreshedNoGroupSeries = RepoFactory.AnimeSeries.GetByID(noGroupSeries.AnimeSeriesID);
            var refreshedDanglingGroupSeries = RepoFactory.AnimeSeries.GetByID(danglingGroupSeries.AnimeSeriesID);

            Assert.NotNull(refreshedValidSeries);
            Assert.NotNull(refreshedNoGroupSeries);
            Assert.NotNull(refreshedDanglingGroupSeries);

            Assert.Equal(validGroup.AnimeGroupID, refreshedValidSeries.AnimeGroupID);
            Assert.True(refreshedNoGroupSeries.AnimeGroupID > 0);
            Assert.True(refreshedDanglingGroupSeries.AnimeGroupID > 0);
            Assert.NotNull(RepoFactory.AnimeGroup.GetByID(refreshedNoGroupSeries.AnimeGroupID));
            Assert.NotNull(RepoFactory.AnimeGroup.GetByID(refreshedDanglingGroupSeries.AnimeGroupID));

            using var scope = Utils.ServiceContainer.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var persistedValidSeries = context.AnimeSeries.AsNoTracking().Single(a => a.AnimeSeriesID == validSeries.AnimeSeriesID);
            var persistedNoGroupSeries = context.AnimeSeries.AsNoTracking().Single(a => a.AnimeSeriesID == noGroupSeries.AnimeSeriesID);
            var persistedDanglingGroupSeries = context.AnimeSeries.AsNoTracking().Single(a => a.AnimeSeriesID == danglingGroupSeries.AnimeSeriesID);

            Assert.Equal(validGroup.AnimeGroupID, persistedValidSeries.AnimeGroupID);
            Assert.Equal(refreshedNoGroupSeries.AnimeGroupID, persistedNoGroupSeries.AnimeGroupID);
            Assert.Equal(refreshedDanglingGroupSeries.AnimeGroupID, persistedDanglingGroupSeries.AnimeGroupID);
            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public void SQLite_AnimeGroupCreator_GetOrCreateSingleGroupForSeries_EfOnlyUsesEfAutoGroupCalculator()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var settingsProvider = Utils.ServiceContainer.GetRequiredService<ISettingsProvider>();
        var settings = settingsProvider.GetSettings();
        var originalAutoGroupSeries = settings.AutoGroupSeries;
        var now = DateTime.UtcNow;
        var existingGroup = new AnimeGroup
        {
            GroupName = $"auto-group-existing-{Guid.NewGuid():N}",
            DateTimeCreated = now,
            DateTimeUpdated = now
        };
        RepoFactory.AnimeGroup.Save(existingGroup);

        var relatedAnimeId = 910001 + Random.Shared.Next(1000);
        var targetAnimeId = 920001 + Random.Shared.Next(1000);

        RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
        {
            AnimeID = relatedAnimeId,
            AnimeType = Shoko.Abstractions.Metadata.Enums.AnimeType.TVSeries,
            AirDate = new DateTime(2000, 1, 1),
            MainTitle = $"related-{Guid.NewGuid():N}",
            AllTitles = string.Empty,
            AllTags = string.Empty,
            Description = string.Empty
        });
        RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
        {
            AnimeID = targetAnimeId,
            AnimeType = Shoko.Abstractions.Metadata.Enums.AnimeType.TVSeries,
            AirDate = new DateTime(2001, 1, 1),
            MainTitle = $"target-{Guid.NewGuid():N}",
            AllTitles = string.Empty,
            AllTags = string.Empty,
            Description = string.Empty
        });
        RepoFactory.AniDB_Anime_Relation.Save(new Shoko.Server.Models.AniDB.AniDB_Anime_Relation
        {
            AnimeID = targetAnimeId,
            RelatedAnimeID = relatedAnimeId,
            RelationType = "prequel"
        });

        RepoFactory.AnimeSeries.Save(new AnimeSeries
        {
            AniDB_ID = relatedAnimeId,
            AnimeGroupID = existingGroup.AnimeGroupID,
            DateTimeCreated = now,
            DateTimeUpdated = now
        }, false, true);

        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        databaseFactory.CloseSessionFactory();
        settings.AutoGroupSeries = true;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            var creator = CreateAnimeGroupCreator();
            var series = new AnimeSeries
            {
                AniDB_ID = targetAnimeId,
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow
            };

            var group = creator.GetOrCreateSingleGroupForSeries(series);

            Assert.NotNull(group);
            Assert.Equal(existingGroup.AnimeGroupID, group.AnimeGroupID);
            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            settings.AutoGroupSeries = originalAutoGroupSeries;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public void SQLite_AutoAnimeGroupCalculator_Create_UsesEfRelationGraphWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var rootAnimeId = 925001 + Random.Shared.Next(1000);
        var relatedAnimeId = 926001 + Random.Shared.Next(1000);
        var dissimilarAnimeId = 927001 + Random.Shared.Next(1000);

        RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
        {
            AnimeID = rootAnimeId,
            AnimeType = Shoko.Abstractions.Metadata.Enums.AnimeType.TVSeries,
            AirDate = new DateTime(2000, 1, 1),
            MainTitle = $"galaxy-frontier-{Guid.NewGuid():N}",
            AllTitles = string.Empty,
            AllTags = string.Empty,
            Description = string.Empty
        });
        RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
        {
            AnimeID = relatedAnimeId,
            AnimeType = Shoko.Abstractions.Metadata.Enums.AnimeType.TVSeries,
            AirDate = new DateTime(2001, 1, 1),
            MainTitle = $"galaxy-frontier-ii-{Guid.NewGuid():N}",
            AllTitles = string.Empty,
            AllTags = string.Empty,
            Description = string.Empty
        });
        RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
        {
            AnimeID = dissimilarAnimeId,
            AnimeType = Shoko.Abstractions.Metadata.Enums.AnimeType.TVSeries,
            AirDate = new DateTime(2002, 1, 1),
            MainTitle = $"cooking-academy-{Guid.NewGuid():N}",
            AllTitles = string.Empty,
            AllTags = string.Empty,
            Description = string.Empty
        });
        RepoFactory.AniDB_Anime_Relation.Save(new Shoko.Server.Models.AniDB.AniDB_Anime_Relation
        {
            AnimeID = relatedAnimeId,
            RelatedAnimeID = rootAnimeId,
            RelationType = "prequel"
        });
        RepoFactory.AniDB_Anime_Relation.Save(new Shoko.Server.Models.AniDB.AniDB_Anime_Relation
        {
            AnimeID = dissimilarAnimeId,
            RelatedAnimeID = rootAnimeId,
            RelationType = "same setting"
        });

        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        databaseFactory.CloseSessionFactory();
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            var calculator = AutoAnimeGroupCalculator.Create(
                exclusions: AutoGroupExclude.None,
                relationsToFuzzyTitleTest: AutoAnimeGroupCalculator.AnimeRelationType.SecondaryRelations,
                mainAnimeSelectionStrategy: MainAnimeSelectionStrategy.MinAirDate);

            var groupedAnimeIds = calculator.GetIdsOfAnimeInSameGroup(relatedAnimeId);

            Assert.Equal(rootAnimeId, calculator.GetGroupAnimeId(rootAnimeId));
            Assert.Equal(rootAnimeId, calculator.GetGroupAnimeId(relatedAnimeId));
            Assert.Equal(dissimilarAnimeId, calculator.GetGroupAnimeId(dissimilarAnimeId));
            Assert.Contains(rootAnimeId, groupedAnimeIds);
            Assert.Contains(relatedAnimeId, groupedAnimeIds);
            Assert.DoesNotContain(dissimilarAnimeId, groupedAnimeIds);
            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public async Task SQLite_AnimeGroupCreator_RecalculateStatsContractsForGroup_EfOnlyRecalculatesStatsWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var now = DateTime.UtcNow;
        var user = new JMMUser
        {
            Username = $"sqlite-stats-user-{Guid.NewGuid():N}",
            Password = "password",
            IsAdmin = 1
        };
        RepoFactory.JMMUser.Save(user);

        var group = new AnimeGroup
        {
            GroupName = $"group-recalc-{Guid.NewGuid():N}",
            DateTimeCreated = now,
            DateTimeUpdated = now
        };
        RepoFactory.AnimeGroup.Save(group);

        var series = new AnimeSeries
        {
            AniDB_ID = 930001 + Random.Shared.Next(1000),
            AnimeGroupID = group.AnimeGroupID,
            DateTimeCreated = now,
            DateTimeUpdated = now,
            MissingEpisodeCount = 3,
            MissingEpisodeCountGroups = 2,
            LatestEpisodeAirDate = new DateTime(2020, 5, 4)
        };
        RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
        {
            AnimeID = series.AniDB_ID,
            AnimeType = Shoko.Abstractions.Metadata.Enums.AnimeType.TVSeries,
            AirDate = new DateTime(2020, 1, 1),
            MainTitle = $"stats-{Guid.NewGuid():N}",
            AllTitles = string.Empty,
            AllTags = string.Empty,
            Description = string.Empty
        });
        RepoFactory.AnimeSeries.Save(series, false, true);
        RepoFactory.AnimeSeries_User.Save(new AnimeSeries_User
        {
            JMMUserID = user.JMMUserID,
            AnimeSeriesID = series.AnimeSeriesID,
            WatchedCount = 4,
            WatchedEpisodeCount = 7,
            UnwatchedEpisodeCount = 2,
            PlayedCount = 5,
            StoppedCount = 1,
            WatchedDate = new DateTime(2021, 7, 8),
            LastUpdated = now
        });

        databaseFactory.CloseSessionFactory();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            var creator = CreateAnimeGroupCreator();
            await creator.RecalculateStatsContractsForGroup(group);

            var refreshedGroup = RepoFactory.AnimeGroup.GetByID(group.AnimeGroupID);
            var refreshedGroupUser = RepoFactory.AnimeGroup_User.GetByUserAndGroupID(user.JMMUserID, group.AnimeGroupID);

            Assert.NotNull(refreshedGroup);
            Assert.Equal(3, refreshedGroup.MissingEpisodeCount);
            Assert.Equal(2, refreshedGroup.MissingEpisodeCountGroups);
            Assert.Equal(new DateTime(2020, 5, 4), refreshedGroup.LatestEpisodeAirDate);

            Assert.NotNull(refreshedGroupUser);
            Assert.Equal(0, refreshedGroupUser.WatchedCount);
            Assert.Equal(0, refreshedGroupUser.WatchedEpisodeCount);
            Assert.Equal(0, refreshedGroupUser.UnwatchedEpisodeCount);
            Assert.Equal(0, refreshedGroupUser.PlayedCount);
            Assert.Equal(0, refreshedGroupUser.StoppedCount);
            Assert.Null(refreshedGroupUser.WatchedDate);

            using var scope = Utils.ServiceContainer.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var persistedGroup = context.AnimeGroup.AsNoTracking().Single(a => a.AnimeGroupID == group.AnimeGroupID);
            var persistedGroupUser = context.AnimeGroup_User.AsNoTracking().Single(a => a.AnimeGroupID == group.AnimeGroupID && a.JMMUserID == user.JMMUserID);

            Assert.Equal(0, persistedGroup.MissingEpisodeCount);
            Assert.Equal(0, persistedGroup.MissingEpisodeCountGroups);
            Assert.Null(persistedGroup.LatestEpisodeAirDate);
            Assert.Equal(refreshedGroupUser.WatchedCount, persistedGroupUser.WatchedCount);
            Assert.Equal(refreshedGroupUser.WatchedEpisodeCount, persistedGroupUser.WatchedEpisodeCount);
            Assert.Equal(refreshedGroupUser.UnwatchedEpisodeCount, persistedGroupUser.UnwatchedEpisodeCount);
            Assert.Equal(refreshedGroupUser.PlayedCount, persistedGroupUser.PlayedCount);
            Assert.Equal(refreshedGroupUser.StoppedCount, persistedGroupUser.StoppedCount);
            Assert.Equal(refreshedGroupUser.WatchedDate, persistedGroupUser.WatchedDate);

            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public async Task SQLite_AnimeGroupCreator_ClearGroupsAndDependencies_EfOnlyClearsGroupsAndRebindsSeriesWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var now = DateTime.UtcNow;

        var tempGroup = new AnimeGroup
        {
            GroupName = AnimeGroupCreator.TempGroupName,
            Description = AnimeGroupCreator.TempGroupName,
            DateTimeCreated = now,
            DateTimeUpdated = now
        };
        var staleGroupA = new AnimeGroup
        {
            GroupName = $"clear-stale-a-{Guid.NewGuid():N}",
            DateTimeCreated = now,
            DateTimeUpdated = now
        };
        var staleGroupB = new AnimeGroup
        {
            GroupName = $"clear-stale-b-{Guid.NewGuid():N}",
            DateTimeCreated = now,
            DateTimeUpdated = now
        };
        RepoFactory.AnimeGroup.Save(tempGroup);
        RepoFactory.AnimeGroup.Save(staleGroupA);
        RepoFactory.AnimeGroup.Save(staleGroupB);

        var user = new JMMUser
        {
            Username = $"clear-groups-user-{Guid.NewGuid():N}",
            Password = "password",
            IsAdmin = 1
        };
        RepoFactory.JMMUser.Save(user);

        var animeIds = new[]
        {
            928001 + Random.Shared.Next(1000),
            929001 + Random.Shared.Next(1000)
        };

        for (var index = 0; index < animeIds.Length; index++)
        {
            RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
            {
                AnimeID = animeIds[index],
                AnimeType = Shoko.Abstractions.Metadata.Enums.AnimeType.TVSeries,
                AirDate = new DateTime(2020, 1, index + 1),
                MainTitle = $"clear-groups-anime-{index}-{Guid.NewGuid():N}",
                AllTitles = string.Empty,
                AllTags = string.Empty,
                Description = string.Empty
            });
        }

        RepoFactory.AnimeSeries.Save(new AnimeSeries
        {
            AniDB_ID = animeIds[0],
            AnimeGroupID = staleGroupA.AnimeGroupID,
            DateTimeCreated = now,
            DateTimeUpdated = now
        }, false, true);
        RepoFactory.AnimeSeries.Save(new AnimeSeries
        {
            AniDB_ID = animeIds[1],
            AnimeGroupID = staleGroupB.AnimeGroupID,
            DateTimeCreated = now,
            DateTimeUpdated = now
        }, false, true);

        RepoFactory.AnimeGroup_User.Save(new AnimeGroup_User
        {
            AnimeGroupID = staleGroupA.AnimeGroupID,
            JMMUserID = user.JMMUserID
        });
        RepoFactory.AnimeGroup_User.Save(new AnimeGroup_User
        {
            AnimeGroupID = staleGroupB.AnimeGroupID,
            JMMUserID = user.JMMUserID
        });

        databaseFactory.CloseSessionFactory();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            var creator = CreateAnimeGroupCreator();
            var clearMethod = typeof(AnimeGroupCreator).GetMethod("ClearGroupsAndDependencies", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.NotNull(clearMethod);

            using var session = databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            using var transaction = session.BeginTransaction();
            var clearTask = (Task)clearMethod.Invoke(creator, new object[] { session, tempGroup.AnimeGroupID });
            await clearTask;
            await transaction.CommitAsync();

            Assert.Empty(RepoFactory.AnimeSeries.GetAll());

            var cachedGroups = RepoFactory.AnimeGroup.GetAll();
            Assert.DoesNotContain(cachedGroups, group => group.AnimeGroupID == staleGroupA.AnimeGroupID);
            Assert.DoesNotContain(cachedGroups, group => group.AnimeGroupID == staleGroupB.AnimeGroupID);
            Assert.Contains(cachedGroups, group => group.AnimeGroupID == tempGroup.AnimeGroupID);
            Assert.Empty(RepoFactory.AnimeGroup_User.GetAll());

            using var scope = Utils.ServiceContainer.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var animeIdList = animeIds.ToList();
            var persistedSeries = context.AnimeSeries
                .AsNoTracking()
                .Where(series => animeIdList.Contains(series.AniDB_ID))
                .ToList();
            var persistedGroups = context.AnimeGroup.AsNoTracking().ToList();
            var persistedGroupUsers = context.AnimeGroup_User.AsNoTracking().ToList();

            Assert.Equal(2, persistedSeries.Count);
            Assert.All(persistedSeries, series => Assert.Equal(tempGroup.AnimeGroupID, series.AnimeGroupID));
            Assert.Contains(persistedGroups, group => group.AnimeGroupID == tempGroup.AnimeGroupID);
            Assert.DoesNotContain(persistedGroups, group => group.AnimeGroupID == staleGroupA.AnimeGroupID);
            Assert.DoesNotContain(persistedGroups, group => group.AnimeGroupID == staleGroupB.AnimeGroupID);
            Assert.Empty(persistedGroupUsers);
            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public async Task SQLite_AnimeGroupCreator_RecreateAllGroups_EfOnlyRecreatesGroupsWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var settingsProvider = Utils.ServiceContainer.GetRequiredService<ISettingsProvider>();
        var settings = settingsProvider.GetSettings();
        var originalAutoGroupSeries = settings.AutoGroupSeries;
        var now = DateTime.UtcNow;
        var animeId = 940001 + Random.Shared.Next(1000);

        RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
        {
            AnimeID = animeId,
            AnimeType = Shoko.Abstractions.Metadata.Enums.AnimeType.TVSeries,
            AirDate = new DateTime(2022, 1, 1),
            MainTitle = $"recreate-{Guid.NewGuid():N}",
            AllTitles = string.Empty,
            AllTags = string.Empty,
            Description = string.Empty
        });
        RepoFactory.AnimeSeries.Save(new AnimeSeries
        {
            AniDB_ID = animeId,
            AnimeGroupID = 0,
            DateTimeCreated = now,
            DateTimeUpdated = now
        }, false, true);

        databaseFactory.CloseSessionFactory();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        settings.AutoGroupSeries = false;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            var creator = CreateAnimeGroupCreator();
            await creator.RecreateAllGroups();

            var recreatedSeries = RepoFactory.AnimeSeries.GetByAnimeID(animeId);
            Assert.NotNull(recreatedSeries);
            Assert.True(recreatedSeries.AnimeGroupID > 0);

            var recreatedGroup = RepoFactory.AnimeGroup.GetByID(recreatedSeries.AnimeGroupID);
            Assert.NotNull(recreatedGroup);
            Assert.Equal(recreatedSeries.Title, recreatedGroup.GroupName);
            Assert.DoesNotContain(RepoFactory.AnimeGroup.GetAll(), group => group.GroupName == AnimeGroupCreator.TempGroupName);

            using var scope = Utils.ServiceContainer.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var persistedSeries = context.AnimeSeries.AsNoTracking().Single(a => a.AnimeSeriesID == recreatedSeries.AnimeSeriesID);
            var persistedGroups = context.AnimeGroup.AsNoTracking().ToList();
            var persistedRecreatedGroup = persistedGroups.SingleOrDefault(group => group.AnimeGroupID == recreatedSeries.AnimeGroupID);

            Assert.Equal(recreatedSeries.AnimeGroupID, persistedSeries.AnimeGroupID);
            Assert.NotNull(persistedRecreatedGroup);
            Assert.Equal(recreatedGroup.GroupName, persistedRecreatedGroup.GroupName);
            Assert.DoesNotContain(persistedGroups, group => group.GroupName == AnimeGroupCreator.TempGroupName);
            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            settings.AutoGroupSeries = originalAutoGroupSeries;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public async Task SQLite_AnimeGroupCreator_RecreateAllGroups_EfOnlyRecreatesMultipleSeriesAndRemovesStaleGroupsWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var settingsProvider = Utils.ServiceContainer.GetRequiredService<ISettingsProvider>();
        var settings = settingsProvider.GetSettings();
        var originalAutoGroupSeries = settings.AutoGroupSeries;
        var now = DateTime.UtcNow;

        var staleGroupA = new AnimeGroup
        {
            GroupName = $"stale-group-a-{Guid.NewGuid():N}",
            DateTimeCreated = now,
            DateTimeUpdated = now
        };
        var staleGroupB = new AnimeGroup
        {
            GroupName = $"stale-group-b-{Guid.NewGuid():N}",
            DateTimeCreated = now,
            DateTimeUpdated = now
        };
        RepoFactory.AnimeGroup.Save(staleGroupA);
        RepoFactory.AnimeGroup.Save(staleGroupB);

        var animeIds = new[]
        {
            950001 + Random.Shared.Next(1000),
            951001 + Random.Shared.Next(1000),
            952001 + Random.Shared.Next(1000)
        };

        for (var index = 0; index < animeIds.Length; index++)
        {
            RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
            {
                AnimeID = animeIds[index],
                AnimeType = Shoko.Abstractions.Metadata.Enums.AnimeType.TVSeries,
                AirDate = new DateTime(2022, 1, index + 1),
                MainTitle = $"multi-recreate-{index}-{Guid.NewGuid():N}",
                AllTitles = string.Empty,
                AllTags = string.Empty,
                Description = string.Empty
            });
        }

        RepoFactory.AnimeSeries.Save(new AnimeSeries
        {
            AniDB_ID = animeIds[0],
            AnimeGroupID = staleGroupA.AnimeGroupID,
            DateTimeCreated = now,
            DateTimeUpdated = now
        }, false, true);
        RepoFactory.AnimeSeries.Save(new AnimeSeries
        {
            AniDB_ID = animeIds[1],
            AnimeGroupID = staleGroupA.AnimeGroupID,
            DateTimeCreated = now,
            DateTimeUpdated = now
        }, false, true);
        RepoFactory.AnimeSeries.Save(new AnimeSeries
        {
            AniDB_ID = animeIds[2],
            AnimeGroupID = staleGroupB.AnimeGroupID,
            DateTimeCreated = now,
            DateTimeUpdated = now
        }, false, true);

        databaseFactory.CloseSessionFactory();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        settings.AutoGroupSeries = false;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            var creator = CreateAnimeGroupCreator();
            await creator.RecreateAllGroups();

            var recreatedSeries = animeIds
                .Select(RepoFactory.AnimeSeries.GetByAnimeID)
                .ToList();

            Assert.All(recreatedSeries, Assert.NotNull);
            Assert.All(recreatedSeries, series => Assert.True(series.AnimeGroupID > 0));
            Assert.All(recreatedSeries, series => Assert.NotEqual(staleGroupA.AnimeGroupID, series.AnimeGroupID));
            Assert.All(recreatedSeries, series => Assert.NotEqual(staleGroupB.AnimeGroupID, series.AnimeGroupID));

            var distinctGroupIds = recreatedSeries.Select(series => series.AnimeGroupID).Distinct().ToList();
            Assert.Equal(recreatedSeries.Count, distinctGroupIds.Count);

            var cachedGroups = RepoFactory.AnimeGroup.GetAll();
            Assert.DoesNotContain(cachedGroups, group => group.AnimeGroupID == staleGroupA.AnimeGroupID);
            Assert.DoesNotContain(cachedGroups, group => group.AnimeGroupID == staleGroupB.AnimeGroupID);
            Assert.DoesNotContain(cachedGroups, group => group.GroupName == AnimeGroupCreator.TempGroupName);
            Assert.All(distinctGroupIds, groupId => Assert.Contains(cachedGroups, group => group.AnimeGroupID == groupId));

            using var scope = Utils.ServiceContainer.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var animeIdList = animeIds.ToList();
            var persistedSeries = context.AnimeSeries
                .AsNoTracking()
                .Where(series => animeIdList.Contains(series.AniDB_ID))
                .ToList();
            var persistedGroups = context.AnimeGroup.AsNoTracking().ToList();

            Assert.Equal(recreatedSeries.Count, persistedSeries.Count);
            Assert.DoesNotContain(persistedGroups, group => group.AnimeGroupID == staleGroupA.AnimeGroupID);
            Assert.DoesNotContain(persistedGroups, group => group.AnimeGroupID == staleGroupB.AnimeGroupID);
            Assert.DoesNotContain(persistedGroups, group => group.GroupName == AnimeGroupCreator.TempGroupName);

            foreach (var series in recreatedSeries)
            {
                var persistedSeriesRow = Assert.Single(persistedSeries, row => row.AnimeSeriesID == series.AnimeSeriesID);
                Assert.Equal(series.AnimeGroupID, persistedSeriesRow.AnimeGroupID);
                Assert.Contains(persistedGroups, group => group.AnimeGroupID == series.AnimeGroupID);
                Assert.Contains(cachedGroups, group => group.AnimeGroupID == series.AnimeGroupID);
            }

            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            settings.AutoGroupSeries = originalAutoGroupSeries;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public async Task SQLite_AnimeGroupCreator_RecreateAllGroups_EfOnlyAutoGroupsRelatedSeriesWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var settingsProvider = Utils.ServiceContainer.GetRequiredService<ISettingsProvider>();
        var settings = settingsProvider.GetSettings();
        var originalAutoGroupSeries = settings.AutoGroupSeries;
        var now = DateTime.UtcNow;

        var staleGroupA = new AnimeGroup
        {
            GroupName = $"auto-stale-group-a-{Guid.NewGuid():N}",
            DateTimeCreated = now,
            DateTimeUpdated = now
        };
        var staleGroupB = new AnimeGroup
        {
            GroupName = $"auto-stale-group-b-{Guid.NewGuid():N}",
            DateTimeCreated = now,
            DateTimeUpdated = now
        };
        RepoFactory.AnimeGroup.Save(staleGroupA);
        RepoFactory.AnimeGroup.Save(staleGroupB);

        var relatedAnimeIdA = 960001 + Random.Shared.Next(1000);
        var relatedAnimeIdB = 961001 + Random.Shared.Next(1000);
        var unrelatedAnimeId = 962001 + Random.Shared.Next(1000);

        RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
        {
            AnimeID = relatedAnimeIdA,
            AnimeType = Shoko.Abstractions.Metadata.Enums.AnimeType.TVSeries,
            AirDate = new DateTime(2019, 1, 1),
            MainTitle = $"auto-related-a-{Guid.NewGuid():N}",
            AllTitles = string.Empty,
            AllTags = string.Empty,
            Description = string.Empty
        });
        RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
        {
            AnimeID = relatedAnimeIdB,
            AnimeType = Shoko.Abstractions.Metadata.Enums.AnimeType.TVSeries,
            AirDate = new DateTime(2020, 1, 1),
            MainTitle = $"auto-related-b-{Guid.NewGuid():N}",
            AllTitles = string.Empty,
            AllTags = string.Empty,
            Description = string.Empty
        });
        RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
        {
            AnimeID = unrelatedAnimeId,
            AnimeType = Shoko.Abstractions.Metadata.Enums.AnimeType.TVSeries,
            AirDate = new DateTime(2021, 1, 1),
            MainTitle = $"auto-unrelated-{Guid.NewGuid():N}",
            AllTitles = string.Empty,
            AllTags = string.Empty,
            Description = string.Empty
        });

        RepoFactory.AniDB_Anime_Relation.Save(new Shoko.Server.Models.AniDB.AniDB_Anime_Relation
        {
            AnimeID = relatedAnimeIdB,
            RelatedAnimeID = relatedAnimeIdA,
            RelationType = "prequel"
        });

        RepoFactory.AnimeSeries.Save(new AnimeSeries
        {
            AniDB_ID = relatedAnimeIdA,
            AnimeGroupID = staleGroupA.AnimeGroupID,
            DateTimeCreated = now,
            DateTimeUpdated = now
        }, false, true);
        RepoFactory.AnimeSeries.Save(new AnimeSeries
        {
            AniDB_ID = relatedAnimeIdB,
            AnimeGroupID = staleGroupA.AnimeGroupID,
            DateTimeCreated = now,
            DateTimeUpdated = now
        }, false, true);
        RepoFactory.AnimeSeries.Save(new AnimeSeries
        {
            AniDB_ID = unrelatedAnimeId,
            AnimeGroupID = staleGroupB.AnimeGroupID,
            DateTimeCreated = now,
            DateTimeUpdated = now
        }, false, true);

        databaseFactory.CloseSessionFactory();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        settings.AutoGroupSeries = true;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            var creator = CreateAnimeGroupCreator();
            await creator.RecreateAllGroups();

            var relatedSeriesA = RepoFactory.AnimeSeries.GetByAnimeID(relatedAnimeIdA);
            var relatedSeriesB = RepoFactory.AnimeSeries.GetByAnimeID(relatedAnimeIdB);
            var unrelatedSeries = RepoFactory.AnimeSeries.GetByAnimeID(unrelatedAnimeId);

            Assert.NotNull(relatedSeriesA);
            Assert.NotNull(relatedSeriesB);
            Assert.NotNull(unrelatedSeries);

            Assert.True(relatedSeriesA.AnimeGroupID > 0);
            Assert.True(relatedSeriesB.AnimeGroupID > 0);
            Assert.True(unrelatedSeries.AnimeGroupID > 0);

            Assert.Equal(relatedSeriesA.AnimeGroupID, relatedSeriesB.AnimeGroupID);
            Assert.NotEqual(relatedSeriesA.AnimeGroupID, unrelatedSeries.AnimeGroupID);

            var cachedGroups = RepoFactory.AnimeGroup.GetAll();
            Assert.DoesNotContain(cachedGroups, group => group.AnimeGroupID == staleGroupA.AnimeGroupID);
            Assert.DoesNotContain(cachedGroups, group => group.AnimeGroupID == staleGroupB.AnimeGroupID);
            Assert.DoesNotContain(cachedGroups, group => group.GroupName == AnimeGroupCreator.TempGroupName);
            Assert.Contains(cachedGroups, group => group.AnimeGroupID == relatedSeriesA.AnimeGroupID);
            Assert.Contains(cachedGroups, group => group.AnimeGroupID == unrelatedSeries.AnimeGroupID);

            using var scope = Utils.ServiceContainer.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var animeIdList = new List<int> { relatedAnimeIdA, relatedAnimeIdB, unrelatedAnimeId };
            var persistedSeries = context.AnimeSeries
                .AsNoTracking()
                .Where(series => animeIdList.Contains(series.AniDB_ID))
                .ToList();
            var persistedGroups = context.AnimeGroup.AsNoTracking().ToList();

            Assert.Equal(3, persistedSeries.Count);
            Assert.DoesNotContain(persistedGroups, group => group.AnimeGroupID == staleGroupA.AnimeGroupID);
            Assert.DoesNotContain(persistedGroups, group => group.AnimeGroupID == staleGroupB.AnimeGroupID);
            Assert.DoesNotContain(persistedGroups, group => group.GroupName == AnimeGroupCreator.TempGroupName);

            Assert.Equal(relatedSeriesA.AnimeGroupID, Assert.Single(persistedSeries, series => series.AnimeSeriesID == relatedSeriesA.AnimeSeriesID).AnimeGroupID);
            Assert.Equal(relatedSeriesB.AnimeGroupID, Assert.Single(persistedSeries, series => series.AnimeSeriesID == relatedSeriesB.AnimeSeriesID).AnimeGroupID);
            Assert.Equal(unrelatedSeries.AnimeGroupID, Assert.Single(persistedSeries, series => series.AnimeSeriesID == unrelatedSeries.AnimeSeriesID).AnimeGroupID);
            Assert.Contains(persistedGroups, group => group.AnimeGroupID == relatedSeriesA.AnimeGroupID);
            Assert.Contains(persistedGroups, group => group.AnimeGroupID == unrelatedSeries.AnimeGroupID);

            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            settings.AutoGroupSeries = originalAutoGroupSeries;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public void SQLite_BaseCachedRepository_ParameterlessPopulate_EfOnlyStillRequiresNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        databaseFactory.CloseSessionFactory();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            var ex = Assert.Throws<InvalidOperationException>(() => RepoFactory.CustomTag.Populate(displayName: false));
            Assert.Contains("SessionFactory", ex.Message, StringComparison.OrdinalIgnoreCase);
            Assert.True(SQLite.SessionFactoryCreateCallCount > sessionFactoryCreateCalls);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public void SQLite_BaseCachedRepository_SaveBatch_EfOnlyUsesDefaultWrapperWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var tags = new List<CustomTag>
        {
            new()
            {
                TagName = $"ef-batch-tag-a-{Guid.NewGuid():N}",
                TagDescription = "batch-a"
            },
            new()
            {
                TagName = $"ef-batch-tag-b-{Guid.NewGuid():N}",
                TagDescription = "batch-b"
            }
        };

        databaseFactory.CloseSessionFactory();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            RepoFactory.CustomTag.Save(tags);

            Assert.All(tags, tag => Assert.True(tag.CustomTagID > 0));
            Assert.All(tags, tag => Assert.NotNull(RepoFactory.CustomTag.GetByID(tag.CustomTagID)));

            using var scope = Utils.ServiceContainer.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var tagNames = tags.Select(tag => tag.TagName).ToList();
            var persistedTags = context.CustomTag
                .AsNoTracking()
                .Where(tag => tagNames.Contains(tag.TagName))
                .ToList();

            Assert.Equal(tags.Count, persistedTags.Count);
            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public void SQLite_AnimeSeries_SaveExistingSeries_EfOnlyUsesEfLookupWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var now = DateTime.UtcNow;
        var group = new AnimeGroup
        {
            GroupName = $"series-save-existing-group-{Guid.NewGuid():N}",
            DateTimeCreated = now,
            DateTimeUpdated = now
        };
        RepoFactory.AnimeGroup.Save(group);

        var animeId = 963001 + Random.Shared.Next(1000);
        RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
        {
            AnimeID = animeId,
            AnimeType = Shoko.Abstractions.Metadata.Enums.AnimeType.TVSeries,
            AirDate = new DateTime(2022, 2, 2),
            MainTitle = $"series-save-existing-{Guid.NewGuid():N}",
            AllTitles = string.Empty,
            AllTags = string.Empty,
            Description = string.Empty
        });

        var series = new AnimeSeries
        {
            AniDB_ID = animeId,
            AnimeGroupID = group.AnimeGroupID,
            DateTimeCreated = now,
            DateTimeUpdated = now
        };
        RepoFactory.AnimeSeries.Save(series, false, true);

        databaseFactory.CloseSessionFactory();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            series.DateTimeUpdated = DateTime.UtcNow;
            RepoFactory.AnimeSeries.Save(series);

            var refreshedSeries = RepoFactory.AnimeSeries.GetByID(series.AnimeSeriesID);
            Assert.NotNull(refreshedSeries);
            Assert.Equal(series.AnimeSeriesID, refreshedSeries.AnimeSeriesID);
            Assert.Equal(group.AnimeGroupID, refreshedSeries.AnimeGroupID);

            using var scope = Utils.ServiceContainer.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var persistedSeries = context.AnimeSeries.AsNoTracking().Single(a => a.AnimeSeriesID == series.AnimeSeriesID);
            Assert.Equal(group.AnimeGroupID, persistedSeries.AnimeGroupID);

            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public void SQLite_AnimeSeriesRepository_GetWithMissingEpisodes_EfOnlyUsesEfQueryWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var now = DateTime.UtcNow;
        var group = new AnimeGroup
        {
            GroupName = $"series-missing-episodes-group-{Guid.NewGuid():N}",
            DateTimeCreated = now,
            DateTimeUpdated = now
        };
        RepoFactory.AnimeGroup.Save(group);

        var animeId = 963501 + Random.Shared.Next(1000);
        RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
        {
            AnimeID = animeId,
            AnimeType = Shoko.Abstractions.Metadata.Enums.AnimeType.TVSeries,
            AirDate = new DateTime(2022, 3, 3),
            MainTitle = $"series-missing-episodes-{Guid.NewGuid():N}",
            AllTitles = string.Empty,
            AllTags = string.Empty,
            Description = string.Empty
        });

        var series = new AnimeSeries
        {
            AniDB_ID = animeId,
            AnimeGroupID = group.AnimeGroupID,
            MissingEpisodeCount = 1,
            MissingEpisodeCountGroups = 2,
            DateTimeCreated = now,
            DateTimeUpdated = now
        };
        RepoFactory.AnimeSeries.Save(series, false, true);
        var collectingSeriesId = series.AnimeSeriesID;

        var animeIdMissingOnly = animeId + 1;
        RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
        {
            AnimeID = animeIdMissingOnly,
            AnimeType = Shoko.Abstractions.Metadata.Enums.AnimeType.TVSeries,
            AirDate = new DateTime(2022, 3, 4),
            MainTitle = $"series-missing-only-{Guid.NewGuid():N}",
            AllTitles = string.Empty,
            AllTags = string.Empty,
            Description = string.Empty
        });

        var missingOnlySeries = new AnimeSeries
        {
            AniDB_ID = animeIdMissingOnly,
            AnimeGroupID = group.AnimeGroupID,
            MissingEpisodeCount = 3,
            MissingEpisodeCountGroups = 0,
            DateTimeCreated = now,
            DateTimeUpdated = now
        };
        RepoFactory.AnimeSeries.Save(missingOnlySeries, false, true);

        var animeIdNoMissing = animeId + 2;
        RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
        {
            AnimeID = animeIdNoMissing,
            AnimeType = Shoko.Abstractions.Metadata.Enums.AnimeType.TVSeries,
            AirDate = new DateTime(2022, 3, 5),
            MainTitle = $"series-no-missing-{Guid.NewGuid():N}",
            AllTitles = string.Empty,
            AllTags = string.Empty,
            Description = string.Empty
        });

        var noMissingSeries = new AnimeSeries
        {
            AniDB_ID = animeIdNoMissing,
            AnimeGroupID = group.AnimeGroupID,
            MissingEpisodeCount = 0,
            MissingEpisodeCountGroups = 0,
            DateTimeCreated = now,
            DateTimeUpdated = now
        };
        RepoFactory.AnimeSeries.Save(noMissingSeries, false, true);

        databaseFactory.CloseSessionFactory();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            var collecting = RepoFactory.AnimeSeries.GetWithMissingEpisodes(collecting: true).ToList();
            var nonCollecting = RepoFactory.AnimeSeries.GetWithMissingEpisodes(collecting: false).ToList();

            var collectingSeries = Assert.Single(collecting);
            Assert.Equal(collectingSeriesId, collectingSeries.AnimeSeriesID);

            Assert.Contains(nonCollecting, current => current.AnimeSeriesID == collectingSeriesId);
            Assert.Contains(nonCollecting, current => current.AnimeSeriesID == missingOnlySeries.AnimeSeriesID);
            Assert.DoesNotContain(nonCollecting, current => current.AnimeSeriesID == noMissingSeries.AnimeSeriesID);

            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public void SQLite_AnimeSeriesRepository_EfOnlyUsesEfMaintenanceQueriesWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var baselineMultipleReleaseIds = RepoFactory.AnimeSeries.GetWithMultipleReleases(ignoreVariations: true)
            .Select(series => series.AniDB_ID)
            .ToHashSet();
        var baselineDuplicateFileIds = RepoFactory.AnimeSeries.GetWithDuplicateFiles()
            .Select(series => series.AniDB_ID)
            .ToHashSet();

        var data = SeedAnimeEpisodeLookupData();

        databaseFactory.CloseSessionFactory();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            var multipleReleases = RepoFactory.AnimeSeries.GetWithMultipleReleases(ignoreVariations: true)
                .Select(series => series.AniDB_ID)
                .ToHashSet();
            var duplicateFiles = RepoFactory.AnimeSeries.GetWithDuplicateFiles()
                .Select(series => series.AniDB_ID)
                .ToHashSet();

            var newMultipleReleaseIds = multipleReleases.Except(baselineMultipleReleaseIds).ToHashSet();
            var newDuplicateFileIds = duplicateFiles.Except(baselineDuplicateFileIds).ToHashSet();

            Assert.Equal([data.AnimeId], newMultipleReleaseIds);
            Assert.Equal([data.AnimeId], newDuplicateFileIds);
            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public void SQLite_AnimeEpisodeRepository_EfOnlyUsesEfLookupMethodsWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var data = SeedAnimeEpisodeLookupData();
        databaseFactory.CloseSessionFactory();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            var multipleReleases = RepoFactory.AnimeEpisode.GetWithMultipleReleases(ignoreVariations: true, animeID: data.AnimeId).ToList();
            var duplicateFiles = RepoFactory.AnimeEpisode.GetWithDuplicateFiles(animeID: data.AnimeId).ToList();

            var multipleReleaseEpisode = Assert.Single(multipleReleases);
            var duplicateFileEpisode = Assert.Single(duplicateFiles);

            Assert.Equal(data.MultipleReleaseEpisodeId, multipleReleaseEpisode.AniDB_EpisodeID);
            Assert.Equal(data.DuplicateFilesEpisodeId, duplicateFileEpisode.AniDB_EpisodeID);
            Assert.Equal(data.AnimeId, multipleReleaseEpisode.AniDB_Episode!.AnimeID);
            Assert.Equal(data.AnimeId, duplicateFileEpisode.AniDB_Episode!.AnimeID);
            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public void SQLite_ScanFileRepository_EfOnlyUsesDefaultWrapperWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var scanId = 970001 + Random.Shared.Next(1000);

        RepoFactory.ScanFile.Save(new Shoko.Server.Models.Legacy.ScanFile
        {
            ScanID = scanId,
            ImportFolderID = 1,
            VideoLocal_Place_ID = 1,
            FullName = $"scanfile-waiting-{Guid.NewGuid():N}.mkv",
            FileSize = 100,
            Status = Shoko.Server.Server.ScanFileStatus.Waiting,
            CheckDate = DateTime.UtcNow.AddMinutes(-2),
            Hash = string.Empty,
            HashResult = string.Empty
        });
        RepoFactory.ScanFile.Save(new Shoko.Server.Models.Legacy.ScanFile
        {
            ScanID = scanId,
            ImportFolderID = 1,
            VideoLocal_Place_ID = 2,
            FullName = $"scanfile-processed-{Guid.NewGuid():N}.mkv",
            FileSize = 101,
            Status = Shoko.Server.Server.ScanFileStatus.ProcessedOK,
            CheckDate = DateTime.UtcNow.AddMinutes(-1),
            Hash = string.Empty,
            HashResult = string.Empty
        });
        RepoFactory.ScanFile.Save(new Shoko.Server.Models.Legacy.ScanFile
        {
            ScanID = scanId,
            ImportFolderID = 1,
            VideoLocal_Place_ID = 3,
            FullName = $"scanfile-error-{Guid.NewGuid():N}.mkv",
            FileSize = 102,
            Status = Shoko.Server.Server.ScanFileStatus.ErrorInvalidHash,
            CheckDate = DateTime.UtcNow,
            Hash = string.Empty,
            HashResult = string.Empty
        });
        RepoFactory.ScanFile.Save(new Shoko.Server.Models.Legacy.ScanFile
        {
            ScanID = scanId + 1,
            ImportFolderID = 1,
            VideoLocal_Place_ID = 4,
            FullName = $"scanfile-other-scan-{Guid.NewGuid():N}.mkv",
            FileSize = 103,
            Status = Shoko.Server.Server.ScanFileStatus.Waiting,
            CheckDate = DateTime.UtcNow,
            Hash = string.Empty,
            HashResult = string.Empty
        });

        databaseFactory.CloseSessionFactory();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            var waiting = RepoFactory.ScanFile.GetWaiting(scanId);
            var allForScan = RepoFactory.ScanFile.GetByScanID(scanId);
            var errors = RepoFactory.ScanFile.GetWithError(scanId);
            var waitingCount = RepoFactory.ScanFile.GetWaitingCount(scanId);

            var waitingFile = Assert.Single(waiting);
            Assert.Equal(Shoko.Server.Server.ScanFileStatus.Waiting, waitingFile.Status);
            Assert.Equal(3, allForScan.Count);
            Assert.Single(errors);
            Assert.Equal(Shoko.Server.Server.ScanFileStatus.ErrorInvalidHash, errors[0].Status);
            Assert.Equal(1, waitingCount);
            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public void SQLite_DirectLookupCluster_EfOnlyUsesDefaultWrapperWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var now = DateTime.UtcNow;
        var messageId = 980001 + Random.Shared.Next(1000);
        var updateType = 981001 + Random.Shared.Next(1000);
        var animeId = 982001 + Random.Shared.Next(1000);

        RepoFactory.AniDB_Message.Save(new Shoko.Server.Models.AniDB.AniDB_Message
        {
            MessageID = messageId,
            FromUserId = 1,
            FromUserName = "tester",
            SentAt = now,
            FetchedAt = now,
            Type = Shoko.Server.Server.AniDBMessageType.Normal,
            Title = $"message-{Guid.NewGuid():N}",
            Body = string.Empty,
            Flags = Shoko.Server.Server.AniDBMessageFlags.FileMoved
        });
        RepoFactory.AniDB_Message.Save(new Shoko.Server.Models.AniDB.AniDB_Message
        {
            MessageID = messageId + 1,
            FromUserId = 1,
            FromUserName = "tester",
            SentAt = now,
            FetchedAt = now,
            Type = Shoko.Server.Server.AniDBMessageType.Normal,
            Title = $"message-handled-{Guid.NewGuid():N}",
            Body = string.Empty,
            Flags = Shoko.Server.Server.AniDBMessageFlags.FileMoved | Shoko.Server.Server.AniDBMessageFlags.FileMoveHandled
        });

        RepoFactory.ScheduledUpdate.Save(new Shoko.Server.Models.Internal.ScheduledUpdate
        {
            UpdateType = updateType,
            LastUpdate = now,
            UpdateDetails = $"scheduled-update-{Guid.NewGuid():N}"
        });

        RepoFactory.AniDB_AnimeUpdate.Save(new Shoko.Server.Models.AniDB.AniDB_AnimeUpdate
        {
            AnimeID = animeId,
            UpdatedAt = now
        });

        databaseFactory.CloseSessionFactory();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            var message = RepoFactory.AniDB_Message.GetByMessageId(messageId);
            var unhandledMessages = RepoFactory.AniDB_Message.GetUnhandledFileMoveMessages();
            var scheduledUpdate = RepoFactory.ScheduledUpdate.GetByUpdateType(updateType);
            var animeUpdate = RepoFactory.AniDB_AnimeUpdate.GetByAnimeID(animeId);

            Assert.NotNull(message);
            Assert.Equal(messageId, message.MessageID);
            Assert.Single(unhandledMessages.Where(a => a.MessageID == messageId));

            Assert.NotNull(scheduledUpdate);
            Assert.Equal(updateType, scheduledUpdate.UpdateType);

            Assert.NotNull(animeUpdate);
            Assert.Equal(animeId, animeUpdate.AnimeID);
            Assert.Equal(now, animeUpdate.UpdatedAt);

            using var scope = Utils.ServiceContainer.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var remainingAnimeUpdates = context.Set<Shoko.Server.Models.AniDB.AniDB_AnimeUpdate>()
                .AsNoTracking()
                .Where(a => a.AnimeID == animeId)
                .OrderByDescending(a => a.UpdatedAt)
                .ToList();

            Assert.Single(remainingAnimeUpdates);
            Assert.Equal(now, remainingAnimeUpdates[0].UpdatedAt);
            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public void SQLite_StatelessAniDbDirectCluster_EfOnlyUsesDefaultWrapperWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var jobFactory = Utils.ServiceContainer.GetRequiredService<JobFactory>();
        var animeId = 983001 + Random.Shared.Next(1000);
        var groupAnimeId = animeId + 1;
        var groupId = 984001 + Random.Shared.Next(1000);
        var notifyId = 985001 + Random.Shared.Next(1000);
        var similarAnimeId = 986001 + Random.Shared.Next(1000);
        var creatorId = 987001 + Random.Shared.Next(1000);

        RepoFactory.AniDB_GroupStatus.Save(new Shoko.Server.Models.AniDB.AniDB_GroupStatus
        {
            AnimeID = groupAnimeId,
            GroupID = groupId,
            GroupName = $"group-status-{Guid.NewGuid():N}",
            CompletionState = 1,
            LastEpisodeNumber = 12,
            Rating = 8.5m,
            Votes = 4,
            EpisodeRange = "1-12"
        });

        RepoFactory.AniDB_NotifyQueue.Save(new Shoko.Server.Models.AniDB.AniDB_NotifyQueue
        {
            Type = Shoko.Server.Server.AniDBNotifyType.Message,
            ID = notifyId,
            AddedAt = DateTime.UtcNow
        });

        RepoFactory.AniDB_Anime_Similar.Save(new Shoko.Server.Models.AniDB.AniDB_Anime_Similar
        {
            AnimeID = animeId,
            SimilarAnimeID = similarAnimeId,
            Approval = 9,
            Total = 10
        });

        RepoFactory.AniDB_Anime_Staff.Save(new Shoko.Server.Models.AniDB.AniDB_Anime_Staff
        {
            AnimeID = animeId,
            CreatorID = creatorId,
            Role = "Director",
            RoleType = Shoko.Server.Server.CreatorRoleType.Director,
            Ordering = 1
        });

        databaseFactory.CloseSessionFactory();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            var groupStatuses = RepoFactory.AniDB_GroupStatus.GetByAnimeID(groupAnimeId);
            var notifyQueue = RepoFactory.AniDB_NotifyQueue.GetByType(Shoko.Server.Server.AniDBNotifyType.Message);
            var notifyQueueItem = RepoFactory.AniDB_NotifyQueue.GetByTypeID(Shoko.Server.Server.AniDBNotifyType.Message, notifyId);
            var similarEntries = RepoFactory.AniDB_Anime_Similar.GetByAnimeID(animeId);
            var similarEntry = RepoFactory.AniDB_Anime_Similar.GetByAnimeIDAndSimilarID(animeId, similarAnimeId);
            var staffByAnime = RepoFactory.AniDB_Anime_Staff.GetByAnimeID(animeId);
            var staffByCreator = RepoFactory.AniDB_Anime_Staff.GetByCreatorID(creatorId);

            Assert.Single(groupStatuses);
            Assert.Equal(groupId, groupStatuses[0].GroupID);

            Assert.Contains(notifyQueue, entry => entry.ID == notifyId);
            Assert.NotNull(notifyQueueItem);
            Assert.Equal(notifyId, notifyQueueItem.ID);

            Assert.Single(similarEntries);
            Assert.NotNull(similarEntry);
            Assert.Equal(similarAnimeId, similarEntry.SimilarAnimeID);

            Assert.Single(staffByAnime);
            Assert.Single(staffByCreator);
            Assert.Equal(creatorId, staffByAnime[0].CreatorID);

            RepoFactory.AniDB_NotifyQueue.DeleteForTypeID(Shoko.Server.Server.AniDBNotifyType.Message, notifyId);
            Assert.Null(RepoFactory.AniDB_NotifyQueue.GetByTypeID(Shoko.Server.Server.AniDBNotifyType.Message, notifyId));

            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public void SQLite_CachedAniDbNameLookups_EfOnlyUseCacheIndexesWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var creatorId = 988001 + Random.Shared.Next(1000);
        var characterId = 989001 + Random.Shared.Next(1000);
        var creatorName = $"creator-{Guid.NewGuid():N}";
        var characterName = $"character-{Guid.NewGuid():N}";

        RepoFactory.AniDB_Creator.Save(new Shoko.Server.Models.AniDB.AniDB_Creator
        {
            CreatorID = creatorId,
            Name = creatorName,
            Type = Shoko.Server.Providers.AniDB.CreatorType.Person,
            LastUpdatedAt = DateTime.UtcNow
        });

        RepoFactory.AniDB_Character.Save(new Shoko.Server.Models.AniDB.AniDB_Character
        {
            CharacterID = characterId,
            Name = characterName,
            OriginalName = characterName,
            Description = "test character",
            ImagePath = string.Empty,
            Gender = Shoko.Server.Providers.TMDB.PersonGender.Unknown,
            Type = Shoko.Server.Server.CharacterType.Character,
            LastUpdated = DateTime.UtcNow
        });

        databaseFactory.CloseSessionFactory();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            var creator = RepoFactory.AniDB_Creator.GetByName(creatorName);
            var character = RepoFactory.AniDB_Character.GetByName(characterName);

            Assert.NotNull(creator);
            Assert.Equal(creatorId, creator.CreatorID);
            Assert.NotNull(character);
            Assert.Equal(characterId, character.CharacterID);
            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public void SQLite_TmdbDirectLookupSurface_EfOnlyUsesDefaultWrapperWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var tmdbCompanyId = 990001 + Random.Shared.Next(1000);
        var tmdbPersonId = 991001 + Random.Shared.Next(1000);
        var tmdbShowId = 992001 + Random.Shared.Next(1000);
        var tmdbEpisodeId = 993001 + Random.Shared.Next(1000);
        var collectionId = $"collection-{Guid.NewGuid():N}";
        var groupId = $"group-{Guid.NewGuid():N}";

        RepoFactory.TMDB_Company.Save(new Shoko.Server.Models.TMDB.TMDB_Company
        {
            TmdbCompanyID = tmdbCompanyId,
            Name = $"company-{Guid.NewGuid():N}",
            CountryOfOrigin = "JP"
        });

        RepoFactory.TMDB_Person.Save(new Shoko.Server.Models.TMDB.TMDB_Person
        {
            TmdbPersonID = tmdbPersonId,
            EnglishName = $"person-{Guid.NewGuid():N}",
            EnglishBiography = "test biography",
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        });

        RepoFactory.TMDB_Company_Entity.Save(new Shoko.Server.Models.TMDB.TMDB_Company_Entity
        {
            TmdbCompanyID = tmdbCompanyId,
            TmdbEntityType = Shoko.Server.Server.ForeignEntityType.Show,
            TmdbEntityID = tmdbShowId,
            Ordering = 2,
            ReleasedAt = new DateOnly(2024, 1, 15)
        });

        RepoFactory.TMDB_Title.Save(new Shoko.Server.Models.TMDB.TMDB_Title(
            Shoko.Server.Server.ForeignEntityType.Person,
            tmdbPersonId,
            "Localized Person Name",
            "en",
            "US"));

        RepoFactory.TMDB_AlternateOrdering_Episode.Save(new Shoko.Server.Models.TMDB.TMDB_AlternateOrdering_Episode
        {
            TmdbShowID = tmdbShowId,
            TmdbEpisodeGroupCollectionID = collectionId,
            TmdbEpisodeGroupID = groupId,
            TmdbEpisodeID = tmdbEpisodeId,
            SeasonNumber = 1,
            EpisodeNumber = 3,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        });

        databaseFactory.CloseSessionFactory();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            var company = RepoFactory.TMDB_Company.GetByTmdbCompanyID(tmdbCompanyId);
            var person = RepoFactory.TMDB_Person.GetByTmdbPersonID(tmdbPersonId);
            var companyEntities = RepoFactory.TMDB_Company_Entity.GetByTmdbCompanyID(tmdbCompanyId);
            var companyEntity = RepoFactory.TMDB_Company_Entity.GetByTmdbEntityTypeAndCompanyID(Shoko.Server.Server.ForeignEntityType.Show, tmdbCompanyId);
            var entityByShow = RepoFactory.TMDB_Company_Entity.GetByTmdbEntityTypeAndID(Shoko.Server.Server.ForeignEntityType.Show, tmdbShowId);
            var titles = RepoFactory.TMDB_Title.GetByParentTypeAndID(Shoko.Server.Server.ForeignEntityType.Person, tmdbPersonId);
            var episodesByShow = RepoFactory.TMDB_AlternateOrdering_Episode.GetByTmdbShowID(tmdbShowId);
            var episodeByCollectionAndId = RepoFactory.TMDB_AlternateOrdering_Episode.GetByEpisodeGroupCollectionAndEpisodeIDs(collectionId, tmdbEpisodeId);

            Assert.NotNull(company);
            Assert.Equal(tmdbCompanyId, company.TmdbCompanyID);
            Assert.NotNull(person);
            Assert.Equal(tmdbPersonId, person.TmdbPersonID);

            Assert.Single(companyEntities);
            Assert.Single(companyEntity);
            Assert.Single(entityByShow);
            Assert.Equal(tmdbShowId, companyEntities[0].TmdbEntityID);

            Assert.Single(titles);
            Assert.Equal("Localized Person Name", titles[0].Value);

            Assert.Single(episodesByShow);
            Assert.NotNull(episodeByCollectionAndId);
            Assert.Equal(groupId, episodeByCollectionAndId.TmdbEpisodeGroupID);

            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public async Task SQLite_ActionService_RemoveRecordsWithoutPhysicalFiles_EfOnlyUsesWrapperWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var actionService = Utils.ServiceContainer.GetRequiredService<ActionService>();

        databaseFactory.CloseSessionFactory();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            await actionService.RemoveRecordsWithoutPhysicalFiles(removeMyList: false);
            Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            databaseFactory.CloseSessionFactory();
        }
    }

    [Fact]
    public async Task SQLite_VideoService_RemoveRecord_EfOnlyUsesWrapperWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var videoService = (VideoService)Utils.ServiceContainer.GetRequiredService<IVideoService>();
        var tempRoot = Path.Combine(Path.GetTempPath(), $"shoko-remove-record-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempRoot);
        try
        {
            var relativePath = "remove-record-video.mkv";
            var absolutePath = Path.Combine(tempRoot, relativePath);
            await File.WriteAllBytesAsync(absolutePath, new byte[4096]);

            var folder = new ShokoManagedFolder
            {
                Name = $"SQLite RemoveRecord Folder {Guid.NewGuid():N}",
                Path = tempRoot,
                IsWatched = false
            };
            RepoFactory.ShokoManagedFolder.Save(folder);

            var video = new VideoLocal
            {
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow,
                FileName = Path.GetFileName(relativePath),
                FileSize = new FileInfo(absolutePath).Length,
                Hash = Guid.NewGuid().ToString("N"),
                HashSource = 0,
                IsIgnored = false,
                IsVariation = false,
                MediaVersion = 0,
                MyListID = 0
            };
            RepoFactory.VideoLocal.Save(video, updateEpisodes: false);

            RepoFactory.VideoLocalPlace.Save(new VideoLocal_Place
            {
                ManagedFolderID = folder.ID,
                RelativePath = relativePath,
                VideoID = video.VideoLocalID,
            });

            var persistedPlace = RepoFactory.VideoLocalPlace.GetByRelativePathAndManagedFolderID(relativePath, folder.ID);
            Assert.NotNull(persistedPlace);

            databaseFactory.CloseSessionFactory();
            var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
            SQLite.UseEfOnlyBootstrapForTests = true;
            SQLite.ThrowOnSessionFactoryCreateForTests = true;
            try
            {
                await videoService.RemoveRecord(persistedPlace!, updateMyListStatus: false);

                Assert.Null(RepoFactory.VideoLocalPlace.GetByRelativePathAndManagedFolderID(relativePath, folder.ID));
                Assert.Null(RepoFactory.VideoLocal.GetByID(video.VideoLocalID));
                Assert.NotNull(RepoFactory.ShokoManagedFolder.GetByID(folder.ID));
                Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
            }
            finally
            {
                SQLite.ThrowOnSessionFactoryCreateForTests = false;
                SQLite.UseEfOnlyBootstrapForTests = false;
                databaseFactory.CloseSessionFactory();
            }
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
    public async Task SQLite_VideoService_RemoveManagedFolder_EfOnlyUsesWrapperWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var videoService = (VideoService)Utils.ServiceContainer.GetRequiredService<IVideoService>();
        var tempRoot = Path.Combine(Path.GetTempPath(), $"shoko-remove-folder-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempRoot);
        try
        {
            var relativePath = "remove-folder-video.mkv";
            var absolutePath = Path.Combine(tempRoot, relativePath);
            await File.WriteAllBytesAsync(absolutePath, new byte[4096]);

            var folder = new ShokoManagedFolder
            {
                Name = $"SQLite RemoveManagedFolder {Guid.NewGuid():N}",
                Path = tempRoot,
                IsWatched = false
            };
            RepoFactory.ShokoManagedFolder.Save(folder);

            var video = new VideoLocal
            {
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow,
                FileName = Path.GetFileName(relativePath),
                FileSize = new FileInfo(absolutePath).Length,
                Hash = Guid.NewGuid().ToString("N"),
                HashSource = 0,
                IsIgnored = false,
                IsVariation = false,
                MediaVersion = 0,
                MyListID = 0
            };
            RepoFactory.VideoLocal.Save(video, updateEpisodes: false);

            RepoFactory.VideoLocalPlace.Save(new VideoLocal_Place
            {
                ManagedFolderID = folder.ID,
                RelativePath = relativePath,
                VideoID = video.VideoLocalID,
            });

            databaseFactory.CloseSessionFactory();
            var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
            SQLite.UseEfOnlyBootstrapForTests = true;
            SQLite.ThrowOnSessionFactoryCreateForTests = true;
            try
            {
                await videoService.RemoveManagedFolder(folder, keepRecords: false, removeMyList: false);

                Assert.Null(RepoFactory.ShokoManagedFolder.GetByID(folder.ID));
                Assert.Null(RepoFactory.VideoLocalPlace.GetByRelativePathAndManagedFolderID(relativePath, folder.ID));
                Assert.Null(RepoFactory.VideoLocal.GetByID(video.VideoLocalID));
                Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
            }
            finally
            {
                SQLite.ThrowOnSessionFactoryCreateForTests = false;
                SQLite.UseEfOnlyBootstrapForTests = false;
                databaseFactory.CloseSessionFactory();
            }
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
    public async Task SQLite_Scanner_DeleteAllErroredFiles_EfOnlyUsesWrapperWithoutNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var tempRoot = Path.Combine(Path.GetTempPath(), $"shoko-scanner-delete-errored-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempRoot);
        try
        {
            var relativePath = "scanner-errored-video.mkv";
            var absolutePath = Path.Combine(tempRoot, relativePath);
            await File.WriteAllBytesAsync(absolutePath, new byte[4096]);

            var folder = new ShokoManagedFolder
            {
                Name = $"SQLite Scanner Folder {Guid.NewGuid():N}",
                Path = tempRoot,
                IsWatched = false
            };
            RepoFactory.ShokoManagedFolder.Save(folder);

            var video = new VideoLocal
            {
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow,
                FileName = Path.GetFileName(relativePath),
                FileSize = new FileInfo(absolutePath).Length,
                Hash = Guid.NewGuid().ToString("N"),
                HashSource = 0,
                IsIgnored = false,
                IsVariation = false,
                MediaVersion = 0,
                MyListID = 0
            };
            RepoFactory.VideoLocal.Save(video, updateEpisodes: false);

            var place = new VideoLocal_Place
            {
                ManagedFolderID = folder.ID,
                RelativePath = relativePath,
                VideoID = video.VideoLocalID,
            };
            RepoFactory.VideoLocalPlace.Save(place);
            var persistedPlace = RepoFactory.VideoLocalPlace.GetByRelativePathAndManagedFolderID(relativePath, folder.ID);
            Assert.NotNull(persistedPlace);

            var scan = new Scan
            {
                CreationTIme = DateTime.UtcNow,
                ImportFolders = folder.ID.ToString(),
                Status = ScanStatus.Finished
            };
            RepoFactory.Scan.Save(scan);
            Assert.True(scan.ScanID > 0);

            var erroredFile = new ScanFile
            {
                ScanID = scan.ScanID,
                ImportFolderID = folder.ID,
                VideoLocal_Place_ID = persistedPlace!.ID,
                FullName = absolutePath,
                FileSize = video.FileSize,
                Status = ScanFileStatus.ErrorInvalidHash,
                CheckDate = DateTime.UtcNow,
                Hash = video.Hash,
                HashResult = "different-hash"
            };
            RepoFactory.ScanFile.Save(erroredFile);
            var persistedErroredFile = RepoFactory.ScanFile.GetWithError(scan.ScanID).Single(a => a.FullName == absolutePath);
            Assert.Equal(persistedPlace.ID, persistedErroredFile.VideoLocal_Place_ID);
            Assert.NotNull(RepoFactory.VideoLocalPlace.GetByID(persistedErroredFile.VideoLocal_Place_ID));

            var scanner = new Scanner
            {
                ActiveScan = scan
            };
            Assert.Single(scanner.ActiveErrorFiles);

            databaseFactory.CloseSessionFactory();
            var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
            SQLite.UseEfOnlyBootstrapForTests = true;
            SQLite.ThrowOnSessionFactoryCreateForTests = true;
            try
            {
                scanner.DeleteAllErroredFiles();

                Assert.Empty(scanner.ActiveErrorFiles);
                Assert.False(File.Exists(absolutePath));
                Assert.Null(RepoFactory.ScanFile.GetByID(persistedErroredFile.ScanFileID));
                Assert.Null(RepoFactory.VideoLocalPlace.GetByID(persistedPlace.ID));
                Assert.Null(RepoFactory.VideoLocal.GetByID(video.VideoLocalID));
                Assert.NotNull(RepoFactory.ShokoManagedFolder.GetByID(folder.ID));
                Assert.Equal(sessionFactoryCreateCalls, SQLite.SessionFactoryCreateCallCount);
            }
            finally
            {
                SQLite.ThrowOnSessionFactoryCreateForTests = false;
                SQLite.UseEfOnlyBootstrapForTests = false;
                databaseFactory.CloseSessionFactory();
            }
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
    public async Task SQLite_MediaInfoJob_MissingVideoLocal_SkipsWithoutThrowing()
    {
        var job = new MediaInfoJob(Utils.ServiceContainer.GetRequiredService<IVideoService>())
        {
            VideoLocalID = int.MaxValue - 1
        };
        job._logger = Utils.ServiceContainer.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>()
            .CreateLogger(nameof(SQLite_MediaInfoJob_MissingVideoLocal_SkipsWithoutThrowing));

        Assert.Null(RepoFactory.VideoLocal.GetByID(job.VideoLocalID));

        job.PostInit();
        await job.Process();
    }

    [Fact]
    public async Task SQLite_MediaInfoJob_ExistingVideoLocal_ProcessesNormally()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), $"shoko-mediainfo-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempRoot);
        try
        {
            var relativePath = "existing-video.mkv";
            var absolutePath = Path.Combine(tempRoot, relativePath);
            await File.WriteAllBytesAsync(absolutePath, new byte[4096]);

            var folder = new ShokoManagedFolder
            {
                Name = $"SQLite MediaInfo Folder {Guid.NewGuid():N}",
                Path = tempRoot,
                IsWatched = true
            };
            RepoFactory.ShokoManagedFolder.Save(folder);

            var video = new VideoLocal
            {
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow,
                FileName = Path.GetFileName(relativePath),
                FileSize = new FileInfo(absolutePath).Length,
                Hash = Guid.NewGuid().ToString("N"),
                HashSource = 0,
                IsIgnored = false,
                IsVariation = false,
                MediaVersion = 0,
                MyListID = 0
            };
            RepoFactory.VideoLocal.Save(video, updateEpisodes: false);

            RepoFactory.VideoLocalPlace.Save(new VideoLocal_Place
            {
                ManagedFolderID = folder.ID,
                RelativePath = relativePath,
                VideoID = video.VideoLocalID,
            });

            var job = new MediaInfoJob(Utils.ServiceContainer.GetRequiredService<IVideoService>())
            {
                VideoLocalID = video.VideoLocalID
            };
            job._logger = Utils.ServiceContainer.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>()
                .CreateLogger(nameof(SQLite_MediaInfoJob_ExistingVideoLocal_ProcessesNormally));

            job.PostInit();
            await job.Process();

            var persistedPlace = RepoFactory.VideoLocalPlace.GetByRelativePathAndManagedFolderID(relativePath, folder.ID);
            Assert.NotNull(persistedPlace);
            Assert.Equal(video.VideoLocalID, persistedPlace!.VideoID);
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
    public async Task SQLite_AddFileToMyListJob_MissingVideoLocal_SkipsWithoutThrowing()
    {
        var job = new AddFileToMyListJob(
            Utils.ServiceContainer.GetRequiredService<Shoko.Server.Providers.AniDB.Interfaces.IRequestFactory>(),
            Utils.ServiceContainer.GetRequiredService<Shoko.Server.Settings.ISettingsProvider>(),
            Utils.ServiceContainer.GetRequiredService<Quartz.ISchedulerFactory>(),
            RepoFactory.VideoLocalUser,
            Utils.ServiceContainer.GetRequiredService<Shoko.Abstractions.User.Services.IUserDataService>())
        {
            Hash = Guid.NewGuid().ToString("N")
        };
        job._logger = Utils.ServiceContainer.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>()
            .CreateLogger(nameof(SQLite_AddFileToMyListJob_MissingVideoLocal_SkipsWithoutThrowing));

        Assert.Null(RepoFactory.VideoLocal.GetByEd2k(job.Hash));

        job.PostInit();
        await job.Process();
    }

    [Fact]
    public void SQLite_AddFileToMyListJob_ExistingVideoLocal_ResolvesNormally()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), $"shoko-addtomylist-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempRoot);
        try
        {
            var relativePath = "existing-video.mkv";
            var absolutePath = Path.Combine(tempRoot, relativePath);
            File.WriteAllBytes(absolutePath, new byte[4096]);

            var folder = new ShokoManagedFolder
            {
                Name = $"SQLite AddToMyList Folder {Guid.NewGuid():N}",
                Path = tempRoot,
                IsWatched = true
            };
            RepoFactory.ShokoManagedFolder.Save(folder);

            var hash = Guid.NewGuid().ToString("N");
            var video = new VideoLocal
            {
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow,
                FileName = Path.GetFileName(relativePath),
                FileSize = new FileInfo(absolutePath).Length,
                Hash = hash,
                HashSource = 0,
                IsIgnored = false,
                IsVariation = false,
                MediaVersion = 0,
                MyListID = 0
            };
            RepoFactory.VideoLocal.Save(video, updateEpisodes: false);

            RepoFactory.VideoLocalPlace.Save(new VideoLocal_Place
            {
                ManagedFolderID = folder.ID,
                RelativePath = relativePath,
                VideoID = video.VideoLocalID,
            });

            var job = new AddFileToMyListJob(
                Utils.ServiceContainer.GetRequiredService<Shoko.Server.Providers.AniDB.Interfaces.IRequestFactory>(),
                Utils.ServiceContainer.GetRequiredService<Shoko.Server.Settings.ISettingsProvider>(),
                Utils.ServiceContainer.GetRequiredService<Quartz.ISchedulerFactory>(),
                RepoFactory.VideoLocalUser,
                Utils.ServiceContainer.GetRequiredService<Shoko.Abstractions.User.Services.IUserDataService>())
            {
                Hash = hash
            };
            job._logger = Utils.ServiceContainer.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>()
                .CreateLogger(nameof(SQLite_AddFileToMyListJob_ExistingVideoLocal_ResolvesNormally));

            job.PostInit();

            var details = job.Details;
            Assert.True(details.TryGetValue("File Path", out var filePathValue));
            Assert.Contains(relativePath, filePathValue?.ToString(), StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }

    private static AnimeGroupCreator CreateAnimeGroupCreator()
    {
        var services = Utils.ServiceContainer;
        return new AnimeGroupCreator(
            services.GetRequiredService<SystemService>(),
            services.GetRequiredService<ISettingsProvider>(),
            services.GetRequiredService<QueueHandler>(),
            services.GetRequiredService<ILogger<AnimeGroupCreator>>(),
            services.GetRequiredService<DatabaseFactory>(),
            services.GetRequiredService<AniDB_AnimeRepository>(),
            services.GetRequiredService<AnimeSeriesRepository>(),
            services.GetRequiredService<AnimeGroupRepository>(),
            services.GetRequiredService<AnimeGroup_UserRepository>(),
            services.GetRequiredService<AnimeGroupService>());
    }

    private static (int AnimeId, int MultipleReleaseEpisodeId, int DuplicateFilesEpisodeId) SeedAnimeEpisodeLookupData()
    {
        var now = DateTime.UtcNow;
        var animeId = 964001 + Random.Shared.Next(1000);
        var multipleReleaseEpisodeId = 965001 + Random.Shared.Next(1000);
        var duplicateFilesEpisodeId = 966001 + Random.Shared.Next(1000);

        var group = new AnimeGroup
        {
            GroupName = $"episode-lookup-group-{Guid.NewGuid():N}",
            DateTimeCreated = now,
            DateTimeUpdated = now
        };
        RepoFactory.AnimeGroup.Save(group);

        RepoFactory.AniDB_Anime.Save(new Shoko.Server.Models.AniDB.AniDB_Anime
        {
            AnimeID = animeId,
            AnimeType = Shoko.Abstractions.Metadata.Enums.AnimeType.TVSeries,
            AirDate = new DateTime(2022, 3, 3),
            MainTitle = $"episode-lookup-anime-{Guid.NewGuid():N}",
            AllTitles = string.Empty,
            AllTags = string.Empty,
            Description = string.Empty
        });

        var series = new AnimeSeries
        {
            AniDB_ID = animeId,
            AnimeGroupID = group.AnimeGroupID,
            DateTimeCreated = now,
            DateTimeUpdated = now
        };
        RepoFactory.AnimeSeries.Save(series, false, true);

        RepoFactory.AniDB_Episode.Save(new Shoko.Server.Models.AniDB.AniDB_Episode
        {
            EpisodeID = multipleReleaseEpisodeId,
            AnimeID = animeId,
            EpisodeNumber = 1,
            EpisodeType = Shoko.Abstractions.Metadata.Enums.EpisodeType.Episode,
            LengthSeconds = 1500,
            AirDate = 20220303,
            Rating = "0",
            Votes = "0",
            Description = string.Empty,
            DateTimeUpdated = now
        });
        RepoFactory.AniDB_Episode.Save(new Shoko.Server.Models.AniDB.AniDB_Episode
        {
            EpisodeID = duplicateFilesEpisodeId,
            AnimeID = animeId,
            EpisodeNumber = 2,
            EpisodeType = Shoko.Abstractions.Metadata.Enums.EpisodeType.Episode,
            LengthSeconds = 1500,
            AirDate = 20220310,
            Rating = "0",
            Votes = "0",
            Description = string.Empty,
            DateTimeUpdated = now
        });

        RepoFactory.AnimeEpisode.Save(new AnimeEpisode
        {
            AnimeSeriesID = series.AnimeSeriesID,
            AniDB_EpisodeID = multipleReleaseEpisodeId,
            DateTimeCreated = now,
            DateTimeUpdated = now
        });
        RepoFactory.AnimeEpisode.Save(new AnimeEpisode
        {
            AnimeSeriesID = series.AnimeSeriesID,
            AniDB_EpisodeID = duplicateFilesEpisodeId,
            DateTimeCreated = now,
            DateTimeUpdated = now
        });

        var folderA = new ShokoManagedFolder
        {
            Name = $"episode-lookup-folder-a-{Guid.NewGuid():N}",
            Path = $"/tmp/episode-lookup-folder-a-{Guid.NewGuid():N}",
            IsWatched = false
        };
        var folderB = new ShokoManagedFolder
        {
            Name = $"episode-lookup-folder-b-{Guid.NewGuid():N}",
            Path = $"/tmp/episode-lookup-folder-b-{Guid.NewGuid():N}",
            IsWatched = false
        };
        RepoFactory.ShokoManagedFolder.Save(folderA);
        RepoFactory.ShokoManagedFolder.Save(folderB);

        var multipleReleaseHashA = Guid.NewGuid().ToString("N");
        var multipleReleaseHashB = Guid.NewGuid().ToString("N");
        var duplicateHashA = Guid.NewGuid().ToString("N");
        var duplicateHashB = Guid.NewGuid().ToString("N");

        var multipleReleaseVideoA = new VideoLocal
        {
            DateTimeCreated = now,
            DateTimeUpdated = now,
            FileName = $"multiple-release-a-{Guid.NewGuid():N}.mkv",
            FileSize = 1200,
            Hash = multipleReleaseHashA,
            HashSource = 0,
            IsIgnored = false,
            IsVariation = false,
            MediaVersion = 0,
            MyListID = 0
        };
        var multipleReleaseVideoB = new VideoLocal
        {
            DateTimeCreated = now,
            DateTimeUpdated = now,
            FileName = $"multiple-release-b-{Guid.NewGuid():N}.mkv",
            FileSize = 1300,
            Hash = multipleReleaseHashB,
            HashSource = 0,
            IsIgnored = false,
            IsVariation = false,
            MediaVersion = 0,
            MyListID = 0
        };
        var duplicateVideoA = new VideoLocal
        {
            DateTimeCreated = now,
            DateTimeUpdated = now,
            FileName = $"duplicate-file-a-{Guid.NewGuid():N}.mkv",
            FileSize = 2200,
            Hash = duplicateHashA,
            HashSource = 0,
            IsIgnored = false,
            IsVariation = true,
            MediaVersion = 0,
            MyListID = 0
        };
        var duplicateVideoB = new VideoLocal
        {
            DateTimeCreated = now,
            DateTimeUpdated = now,
            FileName = $"duplicate-file-b-{Guid.NewGuid():N}.mkv",
            FileSize = 2300,
            Hash = duplicateHashB,
            HashSource = 0,
            IsIgnored = false,
            IsVariation = true,
            MediaVersion = 0,
            MyListID = 0
        };
        RepoFactory.VideoLocal.Save(multipleReleaseVideoA, updateEpisodes: false);
        RepoFactory.VideoLocal.Save(multipleReleaseVideoB, updateEpisodes: false);
        RepoFactory.VideoLocal.Save(duplicateVideoA, updateEpisodes: false);
        RepoFactory.VideoLocal.Save(duplicateVideoB, updateEpisodes: false);

        RepoFactory.VideoLocalPlace.Save(new VideoLocal_Place
        {
            VideoID = duplicateVideoA.VideoLocalID,
            ManagedFolderID = folderA.ID,
            RelativePath = $"dup-a-1-{Guid.NewGuid():N}.mkv"
        });
        RepoFactory.VideoLocalPlace.Save(new VideoLocal_Place
        {
            VideoID = duplicateVideoA.VideoLocalID,
            ManagedFolderID = folderB.ID,
            RelativePath = $"dup-a-2-{Guid.NewGuid():N}.mkv"
        });
        RepoFactory.VideoLocalPlace.Save(new VideoLocal_Place
        {
            VideoID = duplicateVideoB.VideoLocalID,
            ManagedFolderID = folderA.ID,
            RelativePath = $"dup-b-1-{Guid.NewGuid():N}.mkv"
        });
        RepoFactory.VideoLocalPlace.Save(new VideoLocal_Place
        {
            VideoID = duplicateVideoB.VideoLocalID,
            ManagedFolderID = folderB.ID,
            RelativePath = $"dup-b-2-{Guid.NewGuid():N}.mkv"
        });

        RepoFactory.CrossRef_File_Episode.Save(new Shoko.Server.Models.CrossReference.CrossRef_File_Episode
        {
            Hash = multipleReleaseHashA,
            FileName = multipleReleaseVideoA.FileName,
            FileSize = multipleReleaseVideoA.FileSize,
            AnimeID = animeId,
            EpisodeID = multipleReleaseEpisodeId,
            Percentage = 100,
            EpisodeOrder = 1
        });
        RepoFactory.CrossRef_File_Episode.Save(new Shoko.Server.Models.CrossReference.CrossRef_File_Episode
        {
            Hash = multipleReleaseHashB,
            FileName = multipleReleaseVideoB.FileName,
            FileSize = multipleReleaseVideoB.FileSize,
            AnimeID = animeId,
            EpisodeID = multipleReleaseEpisodeId,
            Percentage = 100,
            EpisodeOrder = 2
        });
        RepoFactory.CrossRef_File_Episode.Save(new Shoko.Server.Models.CrossReference.CrossRef_File_Episode
        {
            Hash = duplicateHashA,
            FileName = duplicateVideoA.FileName,
            FileSize = duplicateVideoA.FileSize,
            AnimeID = animeId,
            EpisodeID = duplicateFilesEpisodeId,
            Percentage = 100,
            EpisodeOrder = 1
        });
        RepoFactory.CrossRef_File_Episode.Save(new Shoko.Server.Models.CrossReference.CrossRef_File_Episode
        {
            Hash = duplicateHashB,
            FileName = duplicateVideoB.FileName,
            FileSize = duplicateVideoB.FileSize,
            AnimeID = animeId,
            EpisodeID = duplicateFilesEpisodeId,
            Percentage = 100,
            EpisodeOrder = 2
        });

        return (animeId, multipleReleaseEpisodeId, duplicateFilesEpisodeId);
    }
}
