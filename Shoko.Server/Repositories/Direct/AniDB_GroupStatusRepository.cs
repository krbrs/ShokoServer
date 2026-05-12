using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Models.AniDB;
using Shoko.Server.Scheduling;
using Shoko.Server.Scheduling.Jobs.Actions;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Utilities;

namespace Shoko.Server.Repositories.Direct;

public class AniDB_GroupStatusRepository : BaseDirectRepository<AniDB_GroupStatus, int>
{
    private readonly JobFactory _jobFactory;

    public List<AniDB_GroupStatus> GetByAnimeID(int id)
    {
        return Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                return context.Set<AniDB_GroupStatus>()
                    .AsNoTracking()
                    .Where(a => a.AnimeID == id)
                    .ToList();
            }
            
            // Fallback to NHibernate path
            using var session = _databaseFactory.SessionFactory.OpenStatelessSession();
            return session.Query<AniDB_GroupStatus>()
                .Where(a => a.AnimeID == id)
                .ToList();
        });
    }

    public void DeleteForAnime(int animeid)
    {
        Lock(() =>
        {
            // Try EF Core path first if available
            using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
            if (sessionWrapper is EfCoreSessionWrapper efSession)
            {
                using var context = efSession.Context;
                // EF Core: query entities first, then remove them
                var entitiesToDelete = context.Set<AniDB_GroupStatus>()
                    .Where(a => a.AnimeID == animeid)
                    .ToList();
                
                foreach (var entity in entitiesToDelete)
                {
                    context.Remove(entity);
                }
                context.SaveChanges();
            }
            else
            {
                 // Fallback to NHibernate path
                using var session = _databaseFactory.SessionFactory.OpenStatelessSession();
                // Query can't batch delete, while Query can
                session.CreateQuery("DELETE FROM AniDB_GroupStatus WHERE AnimeID = :animeid")
                    .SetParameter("animeid", animeid)
                    .ExecuteUpdate();
            }
        });

        _jobFactory.CreateJob<RefreshAnimeStatsJob>(a => a.AnimeID = animeid).Process().GetAwaiter().GetResult();
    }

    public AniDB_GroupStatusRepository(DatabaseFactory databaseFactory, JobFactory jobFactory) : base(databaseFactory)
    {
        _jobFactory = jobFactory;
    }
}
