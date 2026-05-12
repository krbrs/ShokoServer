using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shoko.Server.Server;
using Shoko.Server.Settings;
using Shoko.Server.Utilities;

using ISettingsProvider = Shoko.Server.Settings.ISettingsProvider;

namespace Benchmarks.T172;

internal static class BenchmarkBootstrapper
{
    public static void Initialize(BenchmarkHarnessSettings settings)
    {
        Utils.ServiceContainer ??= new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        Utils.SettingsProvider = new BenchmarkSettingsProvider(CreateServerSettings(settings));
    }

    private static ServerSettings CreateServerSettings(BenchmarkHarnessSettings settings)
    {
        var serverSettings = new ServerSettings();
        serverSettings.Database.OverrideConnectionString = settings.ConnectionString;
        serverSettings.Database.Type = settings.Provider switch
        {
            BenchmarkProviderType.SQLite => Constants.DatabaseType.SQLite,
            BenchmarkProviderType.MariaDB => Constants.DatabaseType.MySQL,
            BenchmarkProviderType.SQLServer => Constants.DatabaseType.SQLServer,
            _ => Constants.DatabaseType.SQLite,
        };
        return serverSettings;
    }

    private sealed class BenchmarkSettingsProvider(IServerSettings settings) : ISettingsProvider
    {
        public IServerSettings GetSettings(bool copy = false) => settings;

        public void SaveSettings(IServerSettings settings)
        {
        }

        public void SaveSettings()
        {
        }

        public void DebugSettingsToLog()
        {
        }
    }
}
