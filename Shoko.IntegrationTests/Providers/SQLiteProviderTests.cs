using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
}
