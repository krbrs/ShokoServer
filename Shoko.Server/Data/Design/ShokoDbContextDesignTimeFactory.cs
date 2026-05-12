using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Shoko.Server.Data.Design;

/// <summary>
/// Design-time factory for <see cref="ShokoDbContext"/> used by EF Core CLI tooling.
/// 
/// This factory exists solely to unblock migrations tooling (Add-Migration, Update-Database, etc.).
/// It does not integrate with runtime DI, does not read configuration files,
/// and always uses SQLite regardless of the runtime provider.
/// </summary>
public class ShokoDbContextDesignTimeFactory : IDesignTimeDbContextFactory<ShokoDbContext>
{
    public ShokoDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
        optionsBuilder.UseSqlite("Data Source=shoko.db");

        return new ShokoDbContext(optionsBuilder.Options);
    }
}
