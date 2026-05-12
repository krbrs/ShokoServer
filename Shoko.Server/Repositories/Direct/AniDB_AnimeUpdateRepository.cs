using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Models.AniDB;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Utilities;

namespace Shoko.Server.Repositories.Direct;

public class AniDB_AnimeUpdateRepository : BaseDirectRepository<AniDB_AnimeUpdate, int>
{
    public AniDB_AnimeUpdate GetByAnimeID(int id)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                var updates = context.Set<AniDB_AnimeUpdate>()
                    .Where(a => a.AnimeID == id)
                    .OrderByDescending(a => a.UpdatedAt)
                    .AsNoTracking()
                    .ToList();

                var update = updates.FirstOrDefault();
                if (update != null && updates.Count > 1)
                {
                    updates.Remove(update);
                    foreach (var duplicate in updates)
                    {
                        context.Remove(duplicate);
                    }
                    context.SaveChanges();
                }

                return update;
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenSession();
            var cats = session.Query<AniDB_AnimeUpdate>()
                .Where(a => a.AnimeID == id)
                .OrderByDescending(a => a.UpdatedAt).ToList();

            var cat = cats.FirstOrDefault();
            cats.Remove(cat);
            if (cats.Count > 1)
            {
                cats.ForEach(Delete);
            }

            return cat;
        });
    }

    public AniDB_AnimeUpdateRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }


}
