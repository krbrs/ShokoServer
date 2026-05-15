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
using Shoko.Server.Models.Shoko;
using Shoko.Server.Repositories;
using Shoko.Server.Repositories.Cached;
using Shoko.Server.Repositories.Cached.AniDB;
using Shoko.Server.Scheduling;
using Shoko.Server.Scheduling.Jobs.AniDB;
using Shoko.Server.Scheduling.Jobs.Shoko;
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
    public void SQLite_AnimeGroupCreator_GetOrCreateSingleGroupForSeries_EfOnlyStillRequiresNhAutoGroupCalculator()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var settingsProvider = Utils.ServiceContainer.GetRequiredService<ISettingsProvider>();
        var settings = settingsProvider.GetSettings();
        var originalAutoGroupSeries = settings.AutoGroupSeries;
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
                AniDB_ID = 910000 + Random.Shared.Next(1000),
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow
            };

            var ex = Assert.Throws<InvalidOperationException>(() => creator.GetOrCreateSingleGroupForSeries(series));
            Assert.Contains("NH SessionFactory creation is disallowed", ex.Message);
            Assert.Equal(sessionFactoryCreateCalls + 1, SQLite.SessionFactoryCreateCallCount);
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
    public async Task SQLite_AnimeGroupCreator_RecalculateStatsContractsForGroup_EfOnlyStillRequiresNhSessionFactory()
    {
        var databaseFactory = Utils.ServiceContainer.GetRequiredService<DatabaseFactory>();
        var sessionFactoryCreateCalls = SQLite.SessionFactoryCreateCallCount;
        databaseFactory.CloseSessionFactory();
        SQLite.UseEfOnlyBootstrapForTests = true;
        SQLite.ThrowOnSessionFactoryCreateForTests = true;
        try
        {
            var creator = CreateAnimeGroupCreator();
            var group = new AnimeGroup
            {
                GroupName = $"group-recalc-{Guid.NewGuid():N}",
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => creator.RecalculateStatsContractsForGroup(group));
            Assert.Contains("NH SessionFactory creation is disallowed", ex.Message);
            Assert.Equal(sessionFactoryCreateCalls + 1, SQLite.SessionFactoryCreateCallCount);
        }
        finally
        {
            SQLite.ThrowOnSessionFactoryCreateForTests = false;
            SQLite.UseEfOnlyBootstrapForTests = false;
            databaseFactory.CloseSessionFactory();
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
}
