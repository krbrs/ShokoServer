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

public class TMDB_AlternateOrderingRepository : BaseDirectRepository<TMDB_AlternateOrdering, int>
{
    public IReadOnlyList<TMDB_AlternateOrdering> GetByTmdbShowID(int showId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_AlternateOrdering>()
                    .AsNoTracking()
                    .Where(a => a.TmdbShowID == showId)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_AlternateOrdering>()
                .Where(a => a.TmdbShowID == showId)
                .ToList();
        });
    }

    public TMDB_AlternateOrdering? GetByTmdbEpisodeGroupCollectionID(string episodeGroupCollectionId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_AlternateOrdering>()
                    .AsNoTracking()
                    .Where(a => a.TmdbEpisodeGroupCollectionID == episodeGroupCollectionId)
                    .Take(1)
                    .SingleOrDefault();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_AlternateOrdering>()
                .Where(a => a.TmdbEpisodeGroupCollectionID == episodeGroupCollectionId)
                .Take(1)
                .SingleOrDefault();
        });
    }

    public TMDB_AlternateOrdering? GetByEpisodeGroupCollectionAndShowIDs(string collectionId, int showId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_AlternateOrdering>()
                    .AsNoTracking()
                    .Where(a => a.TmdbEpisodeGroupCollectionID == collectionId && a.TmdbShowID == showId)
                    .Take(1)
                    .SingleOrDefault();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_AlternateOrdering>()
                .Where(a => a.TmdbEpisodeGroupCollectionID == collectionId && a.TmdbShowID == showId)
                .Take(1)
                .SingleOrDefault();
        });
    }

    public TMDB_AlternateOrderingRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }
}
