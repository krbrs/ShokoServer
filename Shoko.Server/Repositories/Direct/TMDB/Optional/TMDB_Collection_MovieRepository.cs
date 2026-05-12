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

public class TMDB_Collection_MovieRepository : BaseDirectRepository<TMDB_Collection_Movie, int>
{
    public IReadOnlyList<TMDB_Collection_Movie> GetByTmdbCollectionID(int collectionId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_Collection_Movie>()
                    .AsNoTracking()
                    .Where(a => a.TmdbCollectionID == collectionId)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_Collection_Movie>()
                .Where(a => a.TmdbCollectionID == collectionId)
                .ToList();
        });
    }

    public TMDB_Collection_Movie? GetByTmdbMovieID(int movieId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_Collection_Movie>()
                    .AsNoTracking()
                    .Where(a => a.TmdbMovieID == movieId)
                    .Take(1)
                    .SingleOrDefault();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_Collection_Movie>()
                .Where(a => a.TmdbMovieID == movieId)
                .Take(1)
                .SingleOrDefault();
        });
    }

    public TMDB_Collection_MovieRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }
}
