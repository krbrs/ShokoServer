using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Models.AniDB;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Utilities;

namespace Shoko.Server.Repositories.Direct;

public class AniDB_Anime_SimilarRepository : BaseDirectRepository<AniDB_Anime_Similar, int>
{
    public AniDB_Anime_Similar GetByAnimeIDAndSimilarID(int animeid, int similaranimeid)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<AniDB_Anime_Similar>()
                    .AsNoTracking()
                    .Where(a => a.AnimeID == animeid && a.SimilarAnimeID == similaranimeid)
                    .Take(1)
                    .SingleOrDefault();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenStatelessSession();
            return session.Query<AniDB_Anime_Similar>()
                .Where(a => a.AnimeID == animeid && a.SimilarAnimeID == similaranimeid)
                .Take(1)
                .SingleOrDefault();
        });
    }

    public List<AniDB_Anime_Similar> GetByAnimeID(int id)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<AniDB_Anime_Similar>()
                    .AsNoTracking()
                    .Where(a => a.AnimeID == id)
                    .OrderByDescending(a => a.Approval)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenStatelessSession();
            return session.Query<AniDB_Anime_Similar>()
                .Where(a => a.AnimeID == id)
                .OrderByDescending(a => a.Approval)
                .ToList();
        });
    }

    public AniDB_Anime_SimilarRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }
}
