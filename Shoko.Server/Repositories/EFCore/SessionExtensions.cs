using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Data;
using Shoko.Server.Repositories.NHibernate;

namespace Shoko.Server.Repositories.EFCore;

internal static class SessionExtensions
{
    [DebuggerStepThrough]
    public static ISessionWrapper Wrap(this ShokoDbContext context)
    {
        return new EfCoreSessionWrapper(context);
    }
}