using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Models.Internal;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Utilities;

namespace Shoko.Server.Repositories.Direct;

public class ScheduledUpdateRepository : BaseDirectRepository<ScheduledUpdate, int>
{
    public ScheduledUpdate GetByUpdateType(int uptype)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<ScheduledUpdate>()
                    .AsNoTracking()
                    .Where(a => a.UpdateType == uptype)
                    .Take(1)
                    .SingleOrDefault();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session.Query<ScheduledUpdate>()
                .Where(a => a.UpdateType == uptype)
                .Take(1)
                .SingleOrDefault();
        });
    }

    public ScheduledUpdateRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }
}
