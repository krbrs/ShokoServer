#nullable enable
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Models.TMDB;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Utilities;

namespace Shoko.Server.Repositories.Direct.TMDB.Optional;

public class TMDB_AlternateOrdering_EpisodeRepository : BaseDirectRepository<TMDB_AlternateOrdering_Episode, int>
{
    public IReadOnlyList<TMDB_AlternateOrdering_Episode> GetByTmdbShowID(int showId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_AlternateOrdering_Episode>()
                    .AsNoTracking()
                    .Where(a => a.TmdbShowID == showId)
                    .OrderBy(a => a.TmdbEpisodeGroupCollectionID)
                    .ThenBy(e => e.SeasonNumber == 0)
                    .ThenBy(e => e.SeasonNumber)
                    .ThenBy(xref => xref.EpisodeNumber)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_AlternateOrdering_Episode>()
                .Where(a => a.TmdbShowID == showId)
                .OrderBy(a => a.TmdbEpisodeGroupCollectionID)
                .ThenBy(e => e.SeasonNumber == 0)
                .ThenBy(e => e.SeasonNumber)
                .ThenBy(xref => xref.EpisodeNumber)
                .ToList();
        });
    }

    public IReadOnlyList<TMDB_AlternateOrdering_Episode> GetByTmdbEpisodeGroupCollectionID(string collectionId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_AlternateOrdering_Episode>()
                    .AsNoTracking()
                    .Where(a => a.TmdbEpisodeGroupCollectionID == collectionId)
                    .OrderBy(e => e.SeasonNumber == 0)
                    .ThenBy(e => e.SeasonNumber)
                    .ThenBy(xref => xref.EpisodeNumber)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_AlternateOrdering_Episode>()
                .Where(a => a.TmdbEpisodeGroupCollectionID == collectionId)
                .OrderBy(e => e.SeasonNumber == 0)
                .ThenBy(e => e.SeasonNumber)
                .ThenBy(xref => xref.EpisodeNumber)
                .ToList();
        });
    }

    public IReadOnlyList<TMDB_AlternateOrdering_Episode> GetByTmdbEpisodeGroupID(string groupId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_AlternateOrdering_Episode>()
                    .AsNoTracking()
                    .Where(a => a.TmdbEpisodeGroupID == groupId)
                    .OrderBy(xref => xref.EpisodeNumber)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_AlternateOrdering_Episode>()
                .Where(a => a.TmdbEpisodeGroupID == groupId)
                .OrderBy(xref => xref.EpisodeNumber)
                .ToList();
        });
    }

    public IReadOnlyList<TMDB_AlternateOrdering_Episode> GetByTmdbEpisodeID(int episodeId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_AlternateOrdering_Episode>()
                    .AsNoTracking()
                    .Where(a => a.TmdbEpisodeID == episodeId)
                    .OrderBy(a => a.TmdbEpisodeGroupID)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_AlternateOrdering_Episode>()
                .Where(a => a.TmdbEpisodeID == episodeId)
                .OrderBy(a => a.TmdbEpisodeGroupID)
                .ToList();
        });
    }

    public TMDB_AlternateOrdering_Episode? GetByEpisodeGroupCollectionAndEpisodeIDs(string collectionId, int episodeId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_AlternateOrdering_Episode>()
                    .AsNoTracking()
                    .Where(a => a.TmdbEpisodeGroupCollectionID == collectionId && a.TmdbEpisodeID == episodeId)
                    .OrderBy(a => a.SeasonNumber)
                    .Take(1)
                    .SingleOrDefault();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_AlternateOrdering_Episode>()
                .Where(a => a.TmdbEpisodeGroupCollectionID == collectionId && a.TmdbEpisodeID == episodeId)
                .OrderBy(a => a.SeasonNumber)
                .Take(1)
                .SingleOrDefault();
        });
    }

    public TMDB_AlternateOrdering_EpisodeRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }
}
