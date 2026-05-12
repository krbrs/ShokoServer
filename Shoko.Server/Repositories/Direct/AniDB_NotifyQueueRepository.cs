using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NHibernate.Linq;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Models.AniDB;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Server;
using Shoko.Server.Utilities;

namespace Shoko.Server.Repositories.Direct;

public class AniDB_NotifyQueueRepository : BaseDirectRepository<AniDB_NotifyQueue, int>
{
    public AniDB_NotifyQueue GetByTypeID(AniDBNotifyType type, int id)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<AniDB_NotifyQueue>()
                    .AsNoTracking()
                    .Where(a => a.Type == type && a.ID == id)
                    .Take(1)
                    .SingleOrDefault();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenStatelessSession();
            return session.Query<AniDB_NotifyQueue>()
                .Where(a => a.Type == type && a.ID == id)
                .Take(1)
                .SingleOrDefault();
        });
    }

    public List<AniDB_NotifyQueue> GetByType(AniDBNotifyType type)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<AniDB_NotifyQueue>()
                    .AsNoTracking()
                    .Where(a => a.Type == type)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenStatelessSession();
            return session.Query<AniDB_NotifyQueue>()
                .Where(a => a.Type == type)
                .ToList();
        });
    }

    public void DeleteForTypeID(AniDBNotifyType type, int id)
    {
        Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                var itemsToDelete = context.Set<AniDB_NotifyQueue>()
                    .Where(a => a.Type == type && a.ID == id)
                    .ToList();
                context.RemoveRange(itemsToDelete);
                context.SaveChanges();
                return;
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenStatelessSession();
            // Query can't batch delete, while Query can
            session.Query<AniDB_NotifyQueue>().Where(a => a.Type == type && a.ID == id).Delete();
        });
    }

    public AniDB_NotifyQueueRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }
}
