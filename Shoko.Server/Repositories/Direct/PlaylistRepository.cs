using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NHibernate;
using Shoko.Server.Databases;
using Shoko.Server.Data;
using Shoko.Server.Models.Legacy;
using Shoko.Server.Repositories.NHibernate;
using Shoko.Server.Utilities;

namespace Shoko.Server.Repositories.Direct;

public class PlaylistRepository : BaseDirectRepository<Playlist, int>
{
    public override IReadOnlyList<Playlist> GetAll()
    {
        // Try EF Core path first if available
        using var sessionWrapper = _databaseFactory.OpenSessionWrapper(useEntityFramework: true);
        if (sessionWrapper is EfCoreSessionWrapper efSession)
        {
            using var context = efSession.Context;
            return context.Set<Playlist>()
                .AsNoTracking()
                .OrderBy(a => a.PlaylistName)
                .ToList();
        }
        
        // Fallback to NHibernate path
        return base.GetAll().OrderBy(a => a.PlaylistName).ToList();
    }

    public override IReadOnlyList<Playlist> GetAll(ISession session)
    {
        return base.GetAll(session).OrderBy(a => a.PlaylistName).ToList();
    }

    public override IReadOnlyList<Playlist> GetAll(ISessionWrapper session)
    {
        // Try EF Core path first if available
        if (session is EfCoreSessionWrapper efSession)
        {
            using var context = efSession.Context;
            return context.Set<Playlist>()
                .AsNoTracking()
                .OrderBy(a => a.PlaylistName)
                .ToList();
        }
        
        // Fallback to NHibernate path
        return base.GetAll(session).OrderBy(a => a.PlaylistName).ToList();
    }

    public PlaylistRepository(DatabaseFactory databaseFactory) : base(databaseFactory)
    {
    }
}
