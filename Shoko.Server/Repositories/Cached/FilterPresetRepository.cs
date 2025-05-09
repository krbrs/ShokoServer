using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NutzCode.InMemoryIndex;
using Shoko.Models.Enums;
using Shoko.Server.Databases;
using Shoko.Server.Filters.Functions;
using Shoko.Server.Filters.Info;
using Shoko.Server.Filters.Logic.DateTimes;
using Shoko.Server.Filters.Logic.Expressions;
using Shoko.Server.Filters.Selectors.DateSelectors;
using Shoko.Server.Filters.SortingSelectors;
using Shoko.Server.Filters.User;
using Shoko.Server.Models;
using Shoko.Server.Server;

using Constants = Shoko.Server.Server.Constants;

#nullable enable
namespace Shoko.Server.Repositories.Cached;

public class FilterPresetRepository(DatabaseFactory databaseFactory) : BaseCachedRepository<FilterPreset, int>(databaseFactory)
{
    public static readonly FilterPreset[] DirectoryFilters = [
        new()
        {
            Name = "Seasons", Locked = true, FilterType = GroupFilterType.Season | GroupFilterType.Directory, FilterPresetID = -1
        },
        new()
        {
            Name = "Tags", Locked = true, FilterType = GroupFilterType.Tag | GroupFilterType.Directory, FilterPresetID = -2
        },
        new()
        {
            Name = "Years", Locked = true, FilterType = GroupFilterType.Season | GroupFilterType.Directory, FilterPresetID = -3
        }
    ];

    private PocoIndex<int, FilterPreset, int>? _parentIDs;

    protected override int SelectKey(FilterPreset entity)
        => entity.FilterPresetID;

    public override void PopulateIndexes()
    {
        _parentIDs = Cache.CreateIndex(a => a.ParentFilterPresetID ?? 0);
    }

    private const string Template = "FilterPreset";

    public override void PostProcess()
    {
        // Clean up. This will populate empty conditions and remove duplicate filters
        ServerState.Instance.ServerStartingStatus = "Database - Validating - FilterPreset Cleaning Up Group Filters...";
        var all = GetAll();
        var set = new HashSet<FilterPreset>(all);
        var notIn = all.Except(set).ToList();
        Delete(notIn);
    }

    public void CreateOrVerifyLockedFilters()
    {
        var lockedGFs = GetLockedGroupFilters();

        ServerState.Instance.ServerStartingStatus = $"Database - Validating - FilterPreset Validating Filters...";

        if (!lockedGFs.Any(a => a.Name == Constants.GroupFilterName.ContinueWatching))
        {
            var gf = new FilterPreset
            {
                Name = Constants.GroupFilterName.ContinueWatching,
                Locked = true,
                ApplyAtSeriesLevel = false,
                FilterType = GroupFilterType.None,
                Expression = new AndExpression { Left = new HasWatchedEpisodesExpression(), Right = new HasUnwatchedEpisodesExpression() },
                SortingExpression = new LastWatchedDateSortingSelector { Descending = true }
            };
            Save(gf);
        }

        //Create All filter
        if (!lockedGFs.Any(a => a.Name == Constants.GroupFilterName.All))
        {
            var gf = new FilterPreset
            {
                Name = Constants.GroupFilterName.All,
                Locked = true,
                FilterType = GroupFilterType.All,
                SortingExpression = new NameSortingSelector()
            };
            Save(gf);
        }
    }

    public void CreateInitialFilters()
    {
        // group filters
        // Do to DatabaseFixes, some filters may be made, namely directory filters
        // All, Continue Watching, Years, Seasons, Tags... 6 seems to be enough to tell for now
        // We can't just check the existence of anything specific, as the user can delete most of these
        if (GetTopLevel().Count > 6) return;

        // Favorites
        var gf = new FilterPreset
        {
            Name = Constants.GroupFilterName.Favorites,
            FilterType = GroupFilterType.UserDefined,
            Expression = new IsFavoriteExpression(),
            SortingExpression = new NameSortingSelector()
        };
        Save(gf);

        // Missing Episodes
        gf = new FilterPreset
        {
            Name = Constants.GroupFilterName.MissingEpisodes,
            FilterType = GroupFilterType.UserDefined,
            Expression = new HasMissingEpisodesCollectingExpression(),
            SortingExpression = new MissingEpisodeCollectingCountSortingSelector { Descending = true }
        };
        Save(gf);


        // Newly Added Series
        gf = new FilterPreset
        {
            Name = Constants.GroupFilterName.NewlyAddedSeries,
            FilterType = GroupFilterType.UserDefined,
            Expression = new DateGreaterThanEqualsExpression
            {
                Left = new DateAddFunction(new LastAddedDateSelector(), TimeSpan.FromDays(10)),
                Right = new TodayFunction()
            },
            SortingExpression = new LastAddedDateSortingSelector { Descending = true }
        };
        Save(gf);

        // Newly Airing Series
        gf = new FilterPreset
        {
            Name = Constants.GroupFilterName.NewlyAiringSeries,
            FilterType = GroupFilterType.UserDefined,
            Expression = new DateGreaterThanEqualsExpression(new DateAddFunction(new LastAirDateSelector(), TimeSpan.FromDays(30)), new TodayFunction()),
            SortingExpression = new LastAirDateSortingSelector { Descending = true }
        };
        Save(gf);

        // Votes Needed
        gf = new FilterPreset
        {
            Name = Constants.GroupFilterName.MissingVotes,
            ApplyAtSeriesLevel = true,
            FilterType = GroupFilterType.UserDefined,
            Expression = new AndExpression
            {
                // all watched and none missing
                Left = new AndExpression
                {
                    // all watched, aka no episodes unwatched
                    Left = new NotExpression
                    {
                        Left = new HasUnwatchedEpisodesExpression()
                    },
                    // no missing episodes
                    Right = new NotExpression
                    {
                        Left = new HasMissingEpisodesExpression()
                    }
                },
                // does not have votes
                Right = new NotExpression
                {
                    Left = new HasUserVotesExpression()
                }
            },
            SortingExpression = new NameSortingSelector()
        };
        Save(gf);

        // Recently Watched
        gf = new FilterPreset
        {
            Name = Constants.GroupFilterName.RecentlyWatched,
            FilterType = GroupFilterType.UserDefined,
            Expression = new AndExpression(new HasWatchedEpisodesExpression(), new
                DateGreaterThanEqualsExpression(new DateAddFunction(new LastWatchedDateSelector(), TimeSpan.FromDays(10)), new TodayFunction())),
            SortingExpression = new LastWatchedDateSortingSelector
            {
                Descending = true
            }
        };
        Save(gf);

        // TMDB Link Missing
        gf = new FilterPreset
        {
            Name = Constants.GroupFilterName.MissingLinks,
            ApplyAtSeriesLevel = true,
            FilterType = GroupFilterType.UserDefined,
            Expression = new MissingTmdbLinkExpression(),
            SortingExpression = new NameSortingSelector()
        };
        Save(gf);
    }

    public override void Save(FilterPreset obj)
    {
        WriteLock(() => { base.Save(obj); });
    }

    public override void Save(IReadOnlyCollection<FilterPreset> objs)
    {
        foreach (var obj in objs)
        {
            Save(obj);
        }
    }

    public override void Delete(IReadOnlyCollection<FilterPreset> objs)
    {
        foreach (var cr in objs)
        {
            base.Delete(cr);
        }
    }

    public IReadOnlyList<FilterPreset> GetByParentID(int parentID)
        => ReadLock(() => _parentIDs!.GetMultiple(parentID));

    public FilterPreset? GetTopLevelFilter(int filterID)
    {
        var parent = GetByID(filterID);
        if (parent == null || parent.ParentFilterPresetID is null or 0)
            return parent;

        while (true)
        {
            if (parent.ParentFilterPresetID is null or 0) return parent;
            var next = GetByID(parent.ParentFilterPresetID.Value);
            // should never happen, but it's not completely impossible
            if (next == null) return parent;
            parent = next;
        }
    }

    public IReadOnlyList<FilterPreset> GetTopLevel()
        => GetByParentID(0);

    public IReadOnlyList<FilterPreset> GetLockedGroupFilters()
        => ReadLock(() => Cache.Values.Where(a => a.Locked).ToList());

    public IReadOnlyList<FilterPreset> GetTimeDependentFilters()
        => ReadLock(() => GetAll().Where(a => a.Expression.TimeDependent).ToList());

    public static IReadOnlyList<FilterPreset> GetAllYearFilters(int offset = 0)
        => RepoFactory.AnimeSeries.GetAllYears()
            .Select((s, i) => new FilterPreset
            {
                Name = s.ToString(),
                FilterPresetID = offset - i,
                ParentFilterPresetID = -3,
                FilterType = GroupFilterType.Year,
                Locked = true,
                ApplyAtSeriesLevel = true,
                Expression = new InYearExpression
                {
                    Parameter = s
                },
                SortingExpression = new NameSortingSelector()
            })
            .ToList();

    public static IReadOnlyList<FilterPreset> GetAllSeasonFilters(int offset = 0)
        => RepoFactory.AnimeSeries.GetAllSeasons()
            .Select((season, i) => new FilterPreset
            {
                Name = season.Season + " " + season.Year,
                FilterPresetID = offset - i,
                ParentFilterPresetID = -1,
                Locked = true,
                FilterType = GroupFilterType.Season,
                ApplyAtSeriesLevel = true,
                Expression = new InSeasonExpression { Season = season.Season, Year = season.Year },
                SortingExpression = new NameSortingSelector()
            })
            .ToList();

    public static IReadOnlyList<FilterPreset> GetAllTagFilters(int offset = 0)
    {
        var allTags = new HashSet<string>(
            RepoFactory.AniDB_Tag.GetAllForLocalSeries().Select(a => a.TagName.Replace('`', '\'')),
            StringComparer.InvariantCultureIgnoreCase);
        var info = new CultureInfo("en-US", false).TextInfo;
        return allTags.Select((s, i) => new FilterPreset
        {
            FilterType = GroupFilterType.Tag,
            FilterPresetID = offset - i,
            ParentFilterPresetID = -2,
            ApplyAtSeriesLevel = true,
            Name = info.ToTitleCase(s),
            Locked = true,
            Expression = new HasTagExpression
            {
                Parameter = s
            },
            SortingExpression = new NameSortingSelector()
        }).ToList();
    }

    public IReadOnlyList<FilterPreset> GetAllFiltersForLegacy(bool topLevel = false)
    {
        if (topLevel)
            return GetTopLevel().Concat(DirectoryFilters).ToList();

        var filters = GetAll().ToList();
        filters.AddRange(DirectoryFilters);

        var offset = -4;
        var result = GetAllSeasonFilters(offset);
        offset -= result.Count;
        filters.AddRange(result);
        result = GetAllTagFilters(offset);
        offset -= result.Count;
        filters.AddRange(result);
        result = GetAllYearFilters(offset);
        filters.AddRange(result);
        return filters;
    }
}
