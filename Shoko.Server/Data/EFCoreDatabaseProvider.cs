using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Shoko.Server.Server;

namespace Shoko.Server.Data;

public enum EFCoreDatabaseProvider
{
    SQLite,
    MariaDB,
    SQLServer
}

public static class EFCoreOptionsExtensions
{
    public static EFCoreDatabaseProvider FromDatabaseType(Constants.DatabaseType databaseType)
    {
        return databaseType switch
        {
            Constants.DatabaseType.SQLServer => EFCoreDatabaseProvider.SQLServer,
            Constants.DatabaseType.MySQL => EFCoreDatabaseProvider.MariaDB,
            _ => EFCoreDatabaseProvider.SQLite
        };
    }

    public static void ConfigureShokoDbContext(this DbContextOptionsBuilder options, EFCoreDatabaseProvider provider, string connectionString)
    {
        switch (provider)
        {
            case EFCoreDatabaseProvider.SQLite:
                options.UseSqlite(connectionString);
                break;
            case EFCoreDatabaseProvider.MariaDB:
                options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)));
                break;
            case EFCoreDatabaseProvider.SQLServer:
                options.UseSqlServer(connectionString);
                break;
        }
    }
}
