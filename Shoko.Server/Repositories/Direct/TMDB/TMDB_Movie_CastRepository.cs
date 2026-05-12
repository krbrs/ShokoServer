#nullable enable
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Models.TMDB;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Utilities;

namespace Shoko.Server.Repositories.Direct.TMDB;

public class TMDB_Movie_CastRepository : BaseDirectRepository<TMDB_Movie_Cast, int>
{
    public IReadOnlyList<TMDB_Movie_Cast> GetByTmdbPersonID(int personId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_Movie_Cast>()
                    .AsNoTracking()
                    .Where(a => a.TmdbPersonID == personId)
                    .OrderBy(e => e.TmdbMovieID)
                    .ThenBy(e => e.Ordering)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_Movie_Cast>()
                .Where(a => a.TmdbPersonID == personId)
                .OrderBy(e => e.TmdbMovieID)
                .ThenBy(e => e.Ordering)
                .ToList();
        });
    }

    public IReadOnlyList<TMDB_Movie_Cast> GetByTmdbMovieID(int movieId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_Movie_Cast>()
                    .AsNoTracking()
                    .Where(a => a.TmdbMovieID == movieId)
                    .OrderBy(e => e.Ordering)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_Movie_Cast>()
                .Where(a => a.TmdbMovieID == movieId)
                .OrderBy(e => e.Ordering)
                .ToList();
        });
    }

    public TMDB_Movie_CastRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }
}
