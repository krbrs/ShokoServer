#nullable enable
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Models.TMDB;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Utilities;

namespace Shoko.Server.Repositories.Direct.TMDB.Optional;

public class TMDB_CollectionRepository : BaseDirectRepository<TMDB_Collection, int>
{
    public TMDB_Collection? GetByTmdbCollectionID(int collectionId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_Collection>()
                    .AsNoTracking()
                    .Where(a => a.TmdbCollectionID == collectionId)
                    .Take(1)
                    .SingleOrDefault();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_Collection>()
                .Where(a => a.TmdbCollectionID == collectionId)
                .Take(1)
                .SingleOrDefault();
        });
    }

    public TMDB_CollectionRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }
}
