using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Models.AniDB;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Server;
using Shoko.Server.Utilities;

namespace Shoko.Server.Repositories.Direct;

public class AniDB_MessageRepository : BaseDirectRepository<AniDB_Message, int>
{
    public AniDB_Message GetByMessageId(int id)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<AniDB_Message>()
                    .AsNoTracking()
                    .Where(a => a.MessageID == id)
                    .Take(1)
                    .SingleOrDefault();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session.Query<AniDB_Message>()
                .Where(a => a.MessageID == id)
                .Take(1)
                .SingleOrDefault();
        });
    }

    public List<AniDB_Message> GetUnhandledFileMoveMessages()
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<AniDB_Message>()
                    .AsNoTracking()
                    .Where(a => a.Flags.HasFlag(AniDBMessageFlags.FileMoved) && !a.Flags.HasFlag(AniDBMessageFlags.FileMoveHandled))
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session.Query<AniDB_Message>()
                .Where(a => a.Flags.HasFlag(AniDBMessageFlags.FileMoved) && !a.Flags.HasFlag(AniDBMessageFlags.FileMoveHandled))
                .ToList();
        });
    }

    public AniDB_MessageRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }
}
