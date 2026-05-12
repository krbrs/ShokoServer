#nullable enable
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Models.TMDB;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Utilities;

namespace Shoko.Server.Repositories.Direct.TMDB;

public class TMDB_CompanyRepository : BaseDirectRepository<TMDB_Company, int>
{
    public TMDB_Company? GetByTmdbCompanyID(int companyId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_Company>()
                    .AsNoTracking()
                    .Where(a => a.TmdbCompanyID == companyId)
                    .Take(1)
                    .SingleOrDefault();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_Company>()
                .Where(a => a.TmdbCompanyID == companyId)
                .Take(1)
                .SingleOrDefault();
        });
    }

    public TMDB_CompanyRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }
}
