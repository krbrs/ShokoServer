#nullable enable
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Models.TMDB;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Utilities;

namespace Shoko.Server.Repositories.Direct.TMDB.Optional;

public class TMDB_NetworkRepository : BaseDirectRepository<TMDB_Network, int>
{
    public TMDB_Network? GetByTmdbNetworkID(int tmdbNetworkId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_Network>()
                    .AsNoTracking()
                    .Where(a => a.TmdbNetworkID == tmdbNetworkId)
                    .Take(1)
                    .SingleOrDefault();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_Network>()
                .Where(a => a.TmdbNetworkID == tmdbNetworkId)
                .Take(1)
                .SingleOrDefault();
        });
    }

    public TMDB_NetworkRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }
}
