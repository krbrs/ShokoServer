#nullable enable
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Models.TMDB;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Utilities;

namespace Shoko.Server.Repositories.Direct.TMDB;

public class TMDB_PersonRepository : BaseDirectRepository<TMDB_Person, int>
{
    public TMDB_Person? GetByTmdbPersonID(int creditId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_Person>()
                    .AsNoTracking()
                    .Where(a => a.TmdbPersonID == creditId)
                    .Take(1)
                    .SingleOrDefault();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_Person>()
                .Where(a => a.TmdbPersonID == creditId)
                .Take(1)
                .SingleOrDefault();
        });
    }

    public TMDB_PersonRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }
}
