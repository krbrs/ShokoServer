using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Data;
using Shoko.Server.Repositories.NHibernate;

namespace Shoko.Server.Repositories.EFCore;

internal static class StatelessSessionExtensions
{
    [DebuggerStepThrough]
    public static ISessionWrapper WrapStateless(this ShokoDbContext context)
    {
        // For EF Core, stateless sessions are handled similarly to regular sessions
        // since EF Core doesn't have a separate stateless session concept
        return new EfCoreSessionWrapper(context);
    }
}