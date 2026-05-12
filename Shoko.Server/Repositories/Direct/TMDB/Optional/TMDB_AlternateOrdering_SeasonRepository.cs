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

public class TMDB_AlternateOrdering_SeasonRepository : BaseDirectRepository<TMDB_AlternateOrdering_Season, int>
{
    public IReadOnlyList<TMDB_AlternateOrdering_Season> GetByTmdbShowID(int showId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_AlternateOrdering_Season>()
                    .AsNoTracking()
                    .Where(a => a.TmdbShowID == showId)
                    .OrderBy(a => a.TmdbEpisodeGroupCollectionID)
                    .ThenBy(e => e.SeasonNumber == 0)
                    .ThenBy(e => e.SeasonNumber)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_AlternateOrdering_Season>()
                .Where(a => a.TmdbShowID == showId)
                .OrderBy(a => a.TmdbEpisodeGroupCollectionID)
                .ThenBy(e => e.SeasonNumber == 0)
                .ThenBy(e => e.SeasonNumber)
                .ToList();
        });
    }

    public IReadOnlyList<TMDB_AlternateOrdering_Season> GetByTmdbEpisodeGroupCollectionID(string collectionId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_AlternateOrdering_Season>()
                    .AsNoTracking()
                    .Where(a => a.TmdbEpisodeGroupCollectionID == collectionId)
                    .OrderBy(e => e.SeasonNumber == 0)
                    .ThenBy(e => e.SeasonNumber)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_AlternateOrdering_Season>()
                .Where(a => a.TmdbEpisodeGroupCollectionID == collectionId)
                .OrderBy(e => e.SeasonNumber == 0)
                .ThenBy(e => e.SeasonNumber)
                .ToList();
        });
    }

    public TMDB_AlternateOrdering_Season? GetByTmdbEpisodeGroupID(string groupId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_AlternateOrdering_Season>()
                    .AsNoTracking()
                    .Where(a => a.TmdbEpisodeGroupID == groupId)
                    .Take(1)
                    .SingleOrDefault();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_AlternateOrdering_Season>()
                .Where(a => a.TmdbEpisodeGroupID == groupId)
                .Take(1)
                .SingleOrDefault();
        });
    }

    public TMDB_AlternateOrdering_SeasonRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }
}
