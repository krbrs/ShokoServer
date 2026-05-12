using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Models.AniDB;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Utilities;

namespace Shoko.Server.Repositories.Direct;

public class AniDB_Anime_StaffRepository : BaseDirectRepository<AniDB_Anime_Staff, int>
{
    public List<AniDB_Anime_Staff> GetByAnimeID(int animeID)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<AniDB_Anime_Staff>()
                    .AsNoTracking()
                    .Where(a => a.AnimeID == animeID)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenStatelessSession();
            return session.Query<AniDB_Anime_Staff>()
                .Where(a => a.AnimeID == animeID)
                .ToList();
        });
    }

    public List<AniDB_Anime_Staff> GetByCreatorID(int creatorID)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<AniDB_Anime_Staff>()
                    .AsNoTracking()
                    .Where(a => a.CreatorID == creatorID)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenStatelessSession();
            return session.Query<AniDB_Anime_Staff>()
                .Where(a => a.CreatorID == creatorID)
                .ToList();
        });
    }

    public AniDB_Anime_StaffRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }
}
