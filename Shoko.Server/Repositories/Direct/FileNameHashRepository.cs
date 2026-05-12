using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Models.Shoko;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Utilities;

namespace Shoko.Server.Repositories.Direct;

public class FileNameHashRepository : BaseDirectRepository<FileNameHash, int>
{
    public List<FileNameHash> GetByHash(string hash)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<FileNameHash>()
                    .AsNoTracking()
                    .Where(a => a.Hash == hash)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<FileNameHash>()
                .Where(a => a.Hash == hash)
                .ToList();
        });
    }

    public List<FileNameHash> GetByFileNameAndSize(string filename, long filesize)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<FileNameHash>()
                    .AsNoTracking()
                    .Where(a => a.FileName == filename && a.FileSize == filesize)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<FileNameHash>()
                .Where(a => a.FileName == filename && a.FileSize == filesize)
                .ToList();
        });
    }

    public FileNameHashRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }
}
