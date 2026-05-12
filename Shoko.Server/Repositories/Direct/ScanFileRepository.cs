using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Models.Legacy;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Server;
using Shoko.Server.Utilities;

namespace Shoko.Server.Repositories.Direct;

public class ScanFileRepository(DatabaseFactory databaseFactory) : BaseDirectRepository<ScanFile, int>(databaseFactory)
{
    public List<ScanFile> GetWaiting(int scanID)
        => Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<ScanFile>()
                    .AsNoTracking()
                    .Where(a => a.ScanID == scanID && a.Status == ScanFileStatus.Waiting)
                    .OrderBy(a => a.CheckDate)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session.Query<ScanFile>()
                .Where(a => a.ScanID == scanID && a.Status == ScanFileStatus.Waiting)
                .OrderBy(a => a.CheckDate)
                .ToList();
        });

    public List<ScanFile> GetByScanID(int scanID)
        => Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<ScanFile>()
                    .AsNoTracking()
                    .Where(a => a.ScanID == scanID)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session.Query<ScanFile>()
                .Where(a => a.ScanID == scanID)
                .ToList();
        });

    public List<ScanFile> GetWithError(int scanID)
        => Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<ScanFile>()
                    .AsNoTracking()
                    .Where(a => a.ScanID == scanID && a.Status > ScanFileStatus.ProcessedOK)
                    .OrderBy(a => a.CheckDate)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session.Query<ScanFile>()
                .Where(a => a.ScanID == scanID && a.Status > ScanFileStatus.ProcessedOK)
                .OrderBy(a => a.CheckDate)
                .ToList();
        });

    public int GetWaitingCount(int scanID)
        => Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<ScanFile>()
                    .AsNoTracking()
                    .Count(a => a.ScanID == scanID && a.Status == (int)ScanFileStatus.Waiting);
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session.Query<ScanFile>()
                .Count(a => a.ScanID == scanID && a.Status == (int)ScanFileStatus.Waiting);
        });
}
