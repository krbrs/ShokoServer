using System;
using System.Collections.Generic;
using Namotion.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NJsonSchema;
using Shoko.Abstractions.Config;
using Shoko.Abstractions.Config.Services;
using Shoko.Abstractions.Core;
using Shoko.Abstractions.Metadata.Enums;
using Shoko.Abstractions.Plugin;
using Shoko.Abstractions.Plugin.Models;
using Shoko.Abstractions.Video.Relocation;
using Shoko.Abstractions.Video.Services;
using Shoko.Server.API.v3.Models.Relocation.Input;
using Shoko.Server.API.v3.Services;
using Xunit;

#nullable enable
namespace Shoko.Tests;

public class RelocationApiCoordinatorTests
{
    [Fact]
    public void GetAvailableProviders_UsesPluginScope_WhenPluginIdIsSpecified()
    {
        var pluginId = Guid.NewGuid();
        var plugin = new TestPlugin(pluginId, "Scoped plugin");
        var providerInfo = CreateProviderInfo(plugin, "Scoped provider");

        var pluginManager = new Mock<IPluginManager>();
        pluginManager.Setup(manager => manager.GetPluginInfo(pluginId)).Returns(CreatePluginInfo(pluginId, plugin, "Scoped plugin", isActive: true));

        var videoService = new Mock<IVideoService>();
        var relocationService = new Mock<IVideoRelocationService>();
        relocationService.Setup(service => service.GetProviderInfo(plugin)).Returns([providerInfo]);

        var coordinator = new RelocationApiCoordinator(pluginManager.Object, new Mock<IConfigurationService>().Object, videoService.Object, relocationService.Object);

        var result = coordinator.GetAvailableProviders(new RelocationDiscoveryFilter() { PluginID = pluginId });

        Assert.Single(result);
        Assert.Equal(providerInfo.ID, result[0].ID);
        relocationService.Verify(service => service.GetAvailableProviders(), Times.Never);
        relocationService.Verify(service => service.GetProviderInfo(plugin), Times.Once);
    }

    [Fact]
    public void GetPipeConfiguration_ReturnsNotFound_WhenPipeDoesNotExist()
    {
        var videoService = new Mock<IVideoService>();
        var relocationService = new Mock<IVideoRelocationService>();
        var coordinator = new RelocationApiCoordinator(new Mock<IPluginManager>().Object, new Mock<IConfigurationService>().Object, videoService.Object, relocationService.Object);

        var result = coordinator.GetPipeConfiguration(Guid.NewGuid());

        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        Assert.Contains("not found", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static RelocationProviderInfo CreateProviderInfo(IPlugin plugin, string name)
        => new()
        {
            ID = Guid.NewGuid(),
            Version = new Version(1, 0, 0),
            Name = name,
            Description = name,
            Provider = Mock.Of<IRelocationProvider>(provider =>
                provider.Name == name &&
                provider.Description == name),
            ConfigurationInfo = null,
            PluginInfo = CreatePluginInfo(plugin.ID, plugin, name, isActive: true),
        };

    private static LocalPluginInfo CreatePluginInfo(Guid id, IPlugin? plugin, string name, bool isActive)
        => new()
        {
            ID = id,
            Name = name,
            Description = name,
            Version = new VersionInformation()
            {
                Version = new Version(1, 0, 0),
                RuntimeIdentifier = "any",
                AbstractionVersion = new Version(1, 0, 0),
                SourceRevision = null,
                ReleaseTag = null,
                Channel = ReleaseChannel.Stable,
                ReleasedAt = DateTime.UtcNow,
            },
            Authors = null,
            RepositoryUrl = null,
            HomepageUrl = null,
            Tags = [],
            LoadOrder = 0,
            Thumbnail = null,
            InstalledAt = DateTime.UtcNow,
            IsEnabled = true,
            IsActive = isActive,
            CanLoad = true,
            CanUninstall = true,
            Plugin = plugin,
            PluginType = plugin?.GetType(),
            ServiceRegistrationType = null,
            ApplicationRegistrationType = null,
            ContainingDirectory = null,
            DLLs = [],
            Types = plugin is null ? [] : [plugin.GetType()],
        };

    private sealed class TestPlugin(Guid id, string name) : IPlugin
    {
        public Guid ID { get; } = id;

        public string Name { get; } = name;
    }
}
