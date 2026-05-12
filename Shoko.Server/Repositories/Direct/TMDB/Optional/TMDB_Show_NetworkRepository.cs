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

public class TMDB_Show_NetworkRepository : BaseDirectRepository<TMDB_Show_Network, int>
{
    public IReadOnlyList<TMDB_Show_Network> GetByTmdbNetworkID(int networkId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_Show_Network>()
                    .AsNoTracking()
                    .Where(a => a.TmdbNetworkID == networkId)
                    .OrderBy(e => e.TmdbShowID)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_Show_Network>()
                .Where(a => a.TmdbNetworkID == networkId)
                .OrderBy(e => e.TmdbShowID)
                .ToList();
        });
    }

    public IReadOnlyList<TMDB_Show_Network> GetByTmdbShowID(int showId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_Show_Network>()
                    .AsNoTracking()
                    .Where(a => a.TmdbShowID == showId)
                    .OrderBy(e => e.Ordering)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_Show_Network>()
                .Where(a => a.TmdbShowID == showId)
                .OrderBy(e => e.Ordering)
                .ToList();
        });
    }

    public TMDB_Show_NetworkRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }
}
