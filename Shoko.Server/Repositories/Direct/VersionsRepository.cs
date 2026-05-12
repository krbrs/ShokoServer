using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Models.Internal;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Utilities;

namespace Shoko.Server.Repositories.Direct;

public class VersionsRepository(DatabaseFactory databaseFactory) : BaseDirectRepository<Versions, int>(databaseFactory)
{
    public Dictionary<(string, string), Versions> GetAllByType(string vertype)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<Versions>()
                    .AsNoTracking()
                    .Where(a => a.VersionType == vertype).ToList()
                    .GroupBy(a => (a.VersionValue ?? string.Empty, a.VersionRevision ?? string.Empty))
                    .ToDictionary(a => a.Key, a => a.FirstOrDefault());
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<Versions>()
                .Where(a => a.VersionType == vertype).ToList()
                .GroupBy(a => (a.VersionValue ?? string.Empty, a.VersionRevision ?? string.Empty))
                .ToDictionary(a => a.Key, a => a.FirstOrDefault());
        });
    }
}
