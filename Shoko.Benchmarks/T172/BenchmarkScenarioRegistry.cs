using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NHibernate;
using NHibernate.Linq;
using NHibernateUtil = NHibernate.NHibernateUtil;
using Shoko.Server.Models.AniDB;
using Shoko.Server.Models.CrossReference;
using Shoko.Server.Models.Legacy;
using Shoko.Server.Models.Release;
using Shoko.Server.Models.Shoko;

namespace Benchmarks.T172;

public static class BenchmarkScenarioRegistry
{
    public static readonly IReadOnlyList<BenchmarkScenarioDefinition> All =
    [
        FullTableScan<AnimeSeries>("Q01", "Cache populate AnimeSeries", "Cache materialization", "BaseCachedRepository.Populate via AnimeSeriesRepository"),
        FullTableScan<AnimeEpisode>("Q02", "Cache populate AnimeEpisode", "Cache materialization", "BaseCachedRepository.Populate via AnimeEpisodeRepository"),
        FullTableScan<VideoLocal>("Q03", "Cache populate VideoLocal", "Cache materialization", "BaseCachedRepository.Populate via VideoLocalRepository"),
        FullTableScan<VideoLocal_Place>("Q04", "Cache populate VideoLocal_Place", "Cache materialization", "BaseCachedRepository.Populate via VideoLocal_PlaceRepository"),
        FullTableScan<CrossRef_File_Episode>("Q05", "Cache populate CrossRef_File_Episode", "Cache materialization", "BaseCachedRepository.Populate via CrossRef_File_EpisodeRepository"),
        FullTableScan<StoredReleaseInfo>("Q06", "Cache populate StoredReleaseInfo", "Cache materialization", "BaseCachedRepository.Populate via StoredReleaseInfoRepository"),
        FullTableScan<AniDB_Anime>("Q07", "Cache populate AniDB_Anime", "Cache materialization", "BaseCachedRepository.Populate via AniDB_AnimeRepository"),
        FullTableScan<AniDB_Episode>("Q08", "Cache populate AniDB_Episode", "Cache materialization", "BaseCachedRepository.Populate via AniDB_EpisodeRepository"),
        new("Q09", "Ordered playlist listing", "Operational filtered", "PlaylistRepository.GetAll()", true, "Small ordered listing", ExecuteEfOrderedPlaylists, ExecuteNhOrderedPlaylists),
        new("Q10", "Scan queue waiting rows", "Operational filtered", "ScanFileRepository.GetWaiting(scanId)", true, "Filtered ordered subset", ExecuteEfWaitingScanFiles, ExecuteNhWaitingScanFiles),
        new("Q11", "Scan queue error rows", "Operational filtered", "ScanFileRepository.GetWithError(scanId)", true, "Filtered ordered subset", ExecuteEfErrorScanFiles, ExecuteNhErrorScanFiles),
        new("Q12", "All rows for one scan", "Operational filtered", "ScanFileRepository.GetByScanID(scanId)", true, "One-to-many scan detail set", ExecuteEfScanFilesByScanId, ExecuteNhScanFilesByScanId),
        new("Q13", "Waiting-count aggregate", "Operational filtered", "ScanFileRepository.GetWaitingCount(scanId)", true, "Scalar aggregate count", ExecuteEfWaitingScanFileCount, ExecuteNhWaitingScanFileCount),
        new("Q14", "Relations by anime ID", "Relationship traversal", "AniDB_Anime_RelationRepository.GetByAnimeID(int)", true, "Single anime fan-out query", ExecuteEfRelationsByAnimeId, ExecuteNhRelationsByAnimeId),
        new("Q15", "Batched relations by anime IDs", "Relationship traversal", "AniDB_Anime_RelationRepository.GetByAnimeID(IEnumerable<int>)", true, "Batched IN query", ExecuteEfRelationsByAnimeIds, ExecuteNhRelationsByAnimeIds),
        new("Q16", "Batched reverse relations", "Relationship traversal", "AniDB_Anime_RelationRepository.GetByRelatedAnimeID(IEnumerable<int>)", true, "Reverse batched IN query", ExecuteEfRelationsByRelatedAnimeIds, ExecuteNhRelationsByRelatedAnimeIds),
        new("Q17", "Full linear relation tree expansion", "Relationship traversal", "AniDB_Anime_RelationRepository.GetFullLinearRelationTree(int)", true, "Iterative traversal across multiple DB roundtrips", ExecuteEfFullLinearRelationTree, ExecuteNhFullLinearRelationTree),
        new("Q18", "Series with multiple releases", "Aggregate anomaly", "AnimeSeriesRepository.GetWithMultipleReleases(ignoreVariations)", true, "Aggregate join/group/having query", ExecuteEfSeriesWithMultipleReleases, ExecuteNhSeriesWithMultipleReleases),
        new("Q19", "Episodes with multiple releases", "Aggregate anomaly", "AnimeEpisodeRepository.GetWithMultipleReleases(ignoreVariations, animeId?)", true, "Aggregate join/group/having query", ExecuteEfEpisodesWithMultipleReleases, ExecuteNhEpisodesWithMultipleReleases),
        new("Q20", "Episodes with duplicate files", "Aggregate anomaly", "AnimeEpisodeRepository.GetWithDuplicateFiles(animeId?)", true, "Subquery + join + group-by query", ExecuteEfEpisodesWithDuplicateFiles, ExecuteNhEpisodesWithDuplicateFiles),
    ];

    public static readonly IReadOnlyList<string> AllScenarioIds = All.Select(a => a.Id).ToList();

    private const string MultipleReleasesIgnoreVariationsQuery =
        @"SELECT DISTINCT ani.AnimeID FROM VideoLocal AS vl JOIN CrossRef_File_Episode ani ON vl.Hash = ani.Hash WHERE vl.IsVariation = 0 AND vl.Hash != '' GROUP BY ani.AnimeID, ani.EpisodeID HAVING COUNT(ani.EpisodeID) > 1";

    private const string MultipleEpisodeReleasesIgnoreVariationsQuery =
        @"SELECT ani.EpisodeID FROM VideoLocal AS vl JOIN CrossRef_File_Episode ani ON vl.Hash = ani.Hash WHERE vl.IsVariation = 0 AND vl.Hash != '' GROUP BY ani.EpisodeID HAVING COUNT(ani.EpisodeID) > 1";

    private const string DuplicateFilesQuery = @"
SELECT
    ani.EpisodeID
FROM
    (
        SELECT
            vl.FileSize,
            vl.Hash
        FROM
            VideoLocal AS vl
        WHERE
            VideoLocalID IN (
                SELECT
                    VideoLocalID
                FROM
                    VideoLocal_Place
                GROUP BY
                    VideoLocalID
                HAVING
                    COUNT(VideoLocal_Place_ID) > 1
            )
        AND
            vl.Hash != ''
    ) AS vlp_selected
INNER JOIN
    CrossRef_File_Episode ani
    ON vlp_selected.Hash = ani.Hash
       AND vlp_selected.FileSize = ani.FileSize
GROUP BY
    ani.EpisodeID
";

    public static BenchmarkScenarioDefinition Get(string scenarioId)
        => All.First(a => string.Equals(a.Id, scenarioId, StringComparison.OrdinalIgnoreCase));

    private static BenchmarkScenarioDefinition FullTableScan<TEntity>(string id, string name, string category, string source)
        where TEntity : class
        => new(
            id,
            name,
            category,
            source,
            true,
            "Full materialization query",
            context => context.Set<TEntity>().AsNoTracking().ToList().Count,
            session => session.CreateCriteria(typeof(TEntity)).List<TEntity>().Count);

    private static int ExecuteEfOrderedPlaylists(Shoko.Server.Data.ShokoDbContext context)
        => context.Set<Playlist>().AsNoTracking().OrderBy(a => a.PlaylistName).ToList().Count;

    private static int ExecuteNhOrderedPlaylists(ISession session)
        => session.Query<Playlist>().OrderBy(a => a.PlaylistName).ToList().Count;

    private static int ExecuteEfWaitingScanFiles(Shoko.Server.Data.ShokoDbContext context)
    {
        var scanId = GetAnyScanIdEf(context);
        return context.Set<ScanFile>()
            .AsNoTracking()
            .Where(a => a.ScanID == scanId && a.Status == Shoko.Server.Server.ScanFileStatus.Waiting)
            .OrderBy(a => a.CheckDate)
            .ToList()
            .Count;
    }

    private static int ExecuteNhWaitingScanFiles(ISession session)
    {
        var scanId = GetAnyScanIdNh(session);
        return session.Query<ScanFile>()
            .Where(a => a.ScanID == scanId && a.Status == Shoko.Server.Server.ScanFileStatus.Waiting)
            .OrderBy(a => a.CheckDate)
            .ToList()
            .Count;
    }

    private static int ExecuteEfErrorScanFiles(Shoko.Server.Data.ShokoDbContext context)
    {
        var scanId = GetAnyScanIdEf(context);
        return context.Set<ScanFile>()
            .AsNoTracking()
            .Where(a => a.ScanID == scanId && (int)a.Status > (int)Shoko.Server.Server.ScanFileStatus.ProcessedOK)
            .OrderBy(a => a.CheckDate)
            .ToList()
            .Count;
    }

    private static int ExecuteNhErrorScanFiles(ISession session)
    {
        var scanId = GetAnyScanIdNh(session);
        return session.Query<ScanFile>()
            .Where(a => a.ScanID == scanId && (int)a.Status > (int)Shoko.Server.Server.ScanFileStatus.ProcessedOK)
            .OrderBy(a => a.CheckDate)
            .ToList()
            .Count;
    }

    private static int ExecuteEfScanFilesByScanId(Shoko.Server.Data.ShokoDbContext context)
    {
        var scanId = GetAnyScanIdEf(context);
        return context.Set<ScanFile>().AsNoTracking().Where(a => a.ScanID == scanId).ToList().Count;
    }

    private static int ExecuteNhScanFilesByScanId(ISession session)
    {
        var scanId = GetAnyScanIdNh(session);
        return session.Query<ScanFile>().Where(a => a.ScanID == scanId).ToList().Count;
    }

    private static int ExecuteEfWaitingScanFileCount(Shoko.Server.Data.ShokoDbContext context)
    {
        var scanId = GetAnyScanIdEf(context);
        return context.Set<ScanFile>()
            .AsNoTracking()
            .Count(a => a.ScanID == scanId && a.Status == Shoko.Server.Server.ScanFileStatus.Waiting);
    }

    private static int ExecuteNhWaitingScanFileCount(ISession session)
    {
        var scanId = GetAnyScanIdNh(session);
        return session.Query<ScanFile>()
            .Count(a => a.ScanID == scanId && a.Status == Shoko.Server.Server.ScanFileStatus.Waiting);
    }

    private static int ExecuteEfRelationsByAnimeId(Shoko.Server.Data.ShokoDbContext context)
    {
        var animeId = GetAnyRelationAnimeIdEf(context);
        return context.Set<AniDB_Anime_Relation>().AsNoTracking().Where(a => a.AnimeID == animeId).ToList().Count;
    }

    private static int ExecuteNhRelationsByAnimeId(ISession session)
    {
        var animeId = GetAnyRelationAnimeIdNh(session);
        return session.Query<AniDB_Anime_Relation>().Where(a => a.AnimeID == animeId).ToList().Count;
    }

    private static int ExecuteEfRelationsByAnimeIds(Shoko.Server.Data.ShokoDbContext context)
    {
        var animeIds = GetRelationAnimeIdsEf(context);
        return context.Set<AniDB_Anime_Relation>().AsNoTracking().Where(a => animeIds.Contains(a.AnimeID)).ToList().Count;
    }

    private static int ExecuteNhRelationsByAnimeIds(ISession session)
    {
        var animeIds = GetRelationAnimeIdsNh(session);
        return session.Query<AniDB_Anime_Relation>().Where(a => animeIds.Contains(a.AnimeID)).ToList().Count;
    }

    private static int ExecuteEfRelationsByRelatedAnimeIds(Shoko.Server.Data.ShokoDbContext context)
    {
        var animeIds = GetRelatedAnimeIdsEf(context);
        return context.Set<AniDB_Anime_Relation>().AsNoTracking().Where(a => animeIds.Contains(a.RelatedAnimeID)).ToList().Count;
    }

    private static int ExecuteNhRelationsByRelatedAnimeIds(ISession session)
    {
        var animeIds = GetRelatedAnimeIdsNh(session);
        return session.Query<AniDB_Anime_Relation>().Where(a => animeIds.Contains(a.RelatedAnimeID)).ToList().Count;
    }

    private static int ExecuteEfFullLinearRelationTree(Shoko.Server.Data.ShokoDbContext context)
    {
        var animeId = GetAnyRelationAnimeIdEf(context);
        return GetAllLinearRelationsEf(context, animeId).Count;
    }

    private static int ExecuteNhFullLinearRelationTree(ISession session)
    {
        var animeId = GetAnyRelationAnimeIdNh(session);
        return GetAllLinearRelationsNh(session, animeId).Count;
    }

    private static int ExecuteEfSeriesWithMultipleReleases(Shoko.Server.Data.ShokoDbContext context)
        => context.Set<VideoLocal>()
            .AsNoTracking()
            .Where(a => !a.IsVariation && a.Hash != string.Empty)
            .Join(
                context.Set<CrossRef_File_Episode>().AsNoTracking(),
                video => video.Hash,
                xref => xref.Hash,
                (video, xref) => new { xref.AnimeID, xref.EpisodeID })
            .GroupBy(tuple => new { tuple.AnimeID, tuple.EpisodeID })
            .Where(group => group.Count() > 1)
            .Select(group => group.Key.AnimeID)
            .Distinct()
            .ToList()
            .Count;

    private static int ExecuteNhSeriesWithMultipleReleases(ISession session)
        => session.CreateSQLQuery(MultipleReleasesIgnoreVariationsQuery)
            .AddScalar("AnimeID", NHibernateUtil.Int32)
            .List<int>()
            .Distinct()
            .Count();

    private static int ExecuteEfEpisodesWithMultipleReleases(Shoko.Server.Data.ShokoDbContext context)
        => context.Set<VideoLocal>()
            .AsNoTracking()
            .Where(a => !a.IsVariation && a.Hash != string.Empty)
            .Join(
                context.Set<CrossRef_File_Episode>().AsNoTracking(),
                video => video.Hash,
                xref => xref.Hash,
                (video, xref) => xref.EpisodeID)
            .GroupBy(episodeId => episodeId)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList()
            .Count;

    private static int ExecuteNhEpisodesWithMultipleReleases(ISession session)
        => session.CreateSQLQuery(MultipleEpisodeReleasesIgnoreVariationsQuery)
            .AddScalar("EpisodeID", NHibernateUtil.Int32)
            .List<int>()
            .Count;

    private static int ExecuteEfEpisodesWithDuplicateFiles(Shoko.Server.Data.ShokoDbContext context)
    {
        var duplicateVideoIds = context.Set<VideoLocal_Place>()
            .AsNoTracking()
            .GroupBy(a => a.VideoID)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key);

        return context.Set<VideoLocal>()
            .AsNoTracking()
            .Where(video => duplicateVideoIds.Contains(video.VideoLocalID) && video.Hash != string.Empty)
            .Join(
                context.Set<CrossRef_File_Episode>().AsNoTracking(),
                video => new { video.Hash, video.FileSize },
                xref => new { Hash = xref.Hash, xref.FileSize },
                (video, xref) => xref.EpisodeID)
            .GroupBy(episodeId => episodeId)
            .Select(group => group.Key)
            .ToList()
            .Count;
    }

    private static int ExecuteNhEpisodesWithDuplicateFiles(ISession session)
        => session.CreateSQLQuery(DuplicateFilesQuery)
            .AddScalar("EpisodeID", NHibernateUtil.Int32)
            .List<int>()
            .Count;

    private static int GetAnyScanIdEf(Shoko.Server.Data.ShokoDbContext context)
        => context.Set<ScanFile>().AsNoTracking().OrderBy(a => a.ScanID).Select(a => a.ScanID).FirstOrDefault();

    private static int GetAnyScanIdNh(ISession session)
        => session.Query<ScanFile>().OrderBy(a => a.ScanID).Select(a => a.ScanID).FirstOrDefault();

    private static int GetAnyRelationAnimeIdEf(Shoko.Server.Data.ShokoDbContext context)
        => context.Set<AniDB_Anime_Relation>().AsNoTracking().OrderBy(a => a.AnimeID).Select(a => a.AnimeID).FirstOrDefault();

    private static int GetAnyRelationAnimeIdNh(ISession session)
        => session.Query<AniDB_Anime_Relation>().OrderBy(a => a.AnimeID).Select(a => a.AnimeID).FirstOrDefault();

    private static List<int> GetRelationAnimeIdsEf(Shoko.Server.Data.ShokoDbContext context)
        => context.Set<AniDB_Anime_Relation>().AsNoTracking().OrderBy(a => a.AnimeID).Select(a => a.AnimeID).Distinct().Take(8).ToList();

    private static List<int> GetRelationAnimeIdsNh(ISession session)
        => session.Query<AniDB_Anime_Relation>().OrderBy(a => a.AnimeID).Select(a => a.AnimeID).Distinct().Take(8).ToList();

    private static List<int> GetRelatedAnimeIdsEf(Shoko.Server.Data.ShokoDbContext context)
        => context.Set<AniDB_Anime_Relation>().AsNoTracking().OrderBy(a => a.RelatedAnimeID).Select(a => a.RelatedAnimeID).Distinct().Take(8).ToList();

    private static List<int> GetRelatedAnimeIdsNh(ISession session)
        => session.Query<AniDB_Anime_Relation>().OrderBy(a => a.RelatedAnimeID).Select(a => a.RelatedAnimeID).Distinct().Take(8).ToList();

    private static HashSet<int> GetAllLinearRelationsEf(Shoko.Server.Data.ShokoDbContext context, int animeId)
    {
        var allRelations = new Queue<int>();
        var visitedNodes = new HashSet<int>();
        var resultRelations = new HashSet<int>();
        allRelations.Enqueue(animeId);

        while (allRelations.TryDequeue(out var relation))
        {
            if (!visitedNodes.Add(relation))
            {
                continue;
            }

            var sequels = GetLinearRelationsUnsafeEf(context, relation);
            if (sequels.Count == 0)
            {
                continue;
            }

            foreach (var sequel in sequels)
            {
                allRelations.Enqueue(sequel);
            }

            resultRelations.UnionWith(sequels);
        }

        return resultRelations;
    }

    private static HashSet<int> GetAllLinearRelationsNh(ISession session, int animeId)
    {
        var allRelations = new Queue<int>();
        var visitedNodes = new HashSet<int>();
        var resultRelations = new HashSet<int>();
        allRelations.Enqueue(animeId);

        while (allRelations.TryDequeue(out var relation))
        {
            if (!visitedNodes.Add(relation))
            {
                continue;
            }

            var sequels = GetLinearRelationsUnsafeNh(session, relation);
            if (sequels.Count == 0)
            {
                continue;
            }

            foreach (var sequel in sequels)
            {
                allRelations.Enqueue(sequel);
            }

            resultRelations.UnionWith(sequels);
        }

        return resultRelations;
    }

    private static HashSet<int> GetLinearRelationsUnsafeEf(Shoko.Server.Data.ShokoDbContext context, int id)
    {
        var query = context.Set<AniDB_Anime_Relation>()
            .AsNoTracking()
            .Where(relation =>
                (relation.AnimeID == id || relation.RelatedAnimeID == id) &&
                (relation.RelationType == "Prequel" || relation.RelationType == "Sequel"));
        var animeIds = query.Select(relation => relation.AnimeID).ToList();
        var relatedIds = query.Select(relation => relation.RelatedAnimeID).ToList();
        return new HashSet<int>(animeIds.Concat(relatedIds));
    }

    private static HashSet<int> GetLinearRelationsUnsafeNh(ISession session, int id)
    {
        var query = session.Query<AniDB_Anime_Relation>()
            .Where(relation =>
                (relation.AnimeID == id || relation.RelatedAnimeID == id) &&
                (relation.RelationType == "Prequel" || relation.RelationType == "Sequel"));
        var animeIds = query.Select(relation => relation.AnimeID).ToList();
        var relatedIds = query.Select(relation => relation.RelatedAnimeID).ToList();
        return new HashSet<int>(animeIds.Concat(relatedIds));
    }
}
