using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shoko.Server.Data;
using Xunit;

namespace Shoko.Tests;

public class AnimeGroupSeriesNavigationWarningTests
{
    [Fact]
    public void RepoBackedConvenienceProperties_DoNotEmitAmbiguousNavigationWarnings()
    {
        using var scope = CreateContext(out var warnings);

        Assert.DoesNotContain(warnings, message => message.Contains(nameof(Shoko.Server.Models.Shoko.AnimeGroup.Parent), StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(warnings, message => message.Contains(nameof(Shoko.Server.Models.Shoko.AnimeGroup.TopLevelAnimeGroup), StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(warnings, message => message.Contains(nameof(Shoko.Server.Models.Shoko.AnimeGroup.Children), StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(warnings, message => message.Contains(nameof(Shoko.Server.Models.Shoko.AnimeGroup.AllChildren), StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(warnings, message => message.Contains(nameof(Shoko.Server.Models.Shoko.AnimeGroup.AllGroupsAbove), StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(warnings, message => message.Contains(nameof(Shoko.Server.Models.Shoko.AnimeGroup.Series), StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(warnings, message => message.Contains(nameof(Shoko.Server.Models.Shoko.AnimeGroup.AllSeries), StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(warnings, message => message.Contains(nameof(Shoko.Server.Models.Shoko.AnimeSeries.AnimeGroup), StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(warnings, message => message.Contains(nameof(Shoko.Server.Models.Shoko.AnimeSeries.TopLevelAnimeGroup), StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(warnings, message => message.Contains(nameof(Shoko.Server.Models.Shoko.AnimeSeries.AllGroupsAbove), StringComparison.OrdinalIgnoreCase));
    }

    private static TestScope CreateContext(out List<string> warnings)
    {
        warnings = [];
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
        optionsBuilder.UseSqlite(connection);
        optionsBuilder.LogTo(warnings.Add, LogLevel.Warning);

        var context = new ShokoDbContext(optionsBuilder.Options);
        _ = context.Model;
        return new TestScope(context, connection);
    }

    private sealed class TestScope : IDisposable
    {
        public TestScope(ShokoDbContext context, SqliteConnection connection)
        {
            Context = context;
            Connection = connection;
        }

        public ShokoDbContext Context { get; }
        private SqliteConnection Connection { get; }

        public void Dispose()
        {
            Context.Dispose();
            Connection.Dispose();
        }
    }
}
