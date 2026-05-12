#nullable enable
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Models.TMDB;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Server;
using Shoko.Server.Utilities;

namespace Shoko.Server.Repositories.Direct.TMDB.Text;

public class TMDB_TitleRepository : BaseDirectRepository<TMDB_Title, int>
{
    public IReadOnlyList<TMDB_Title> GetByParentTypeAndID(ForeignEntityType parentType, int parentId)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<TMDB_Title>()
                    .AsNoTracking()
                    .Where(a => a.ParentType == parentType && a.ParentID == parentId)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            return session
                .Query<TMDB_Title>()
                .Where(a => a.ParentType == parentType && a.ParentID == parentId)
                .ToList();
        });
    }

    public TMDB_TitleRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }
}
