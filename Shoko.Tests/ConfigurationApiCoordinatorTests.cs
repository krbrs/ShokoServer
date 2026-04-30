using System;
using System.Collections.Generic;
using System.Linq;
using Namotion.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NJsonSchema;
using Shoko.Abstractions.Config;
using Shoko.Abstractions.Config.Enums;
using Shoko.Abstractions.Config.Services;
using Shoko.Abstractions.Core;
using Shoko.Abstractions.Metadata.Enums;
using Shoko.Abstractions.Plugin;
using Shoko.Abstractions.Plugin.Models;
using Shoko.Abstractions.User;
using Shoko.Server.API.v3.Models.Common;
using Shoko.Server.API.v3.Models.Configuration.Input;
using Shoko.Server.Plugin;
using Shoko.Server.Services.Configuration;
using Xunit;

#nullable enable
namespace Shoko.Tests;

public class ConfigurationApiCoordinatorTests
{
    [Fact]
    public void GetConfigurations_FiltersHiddenEntries()
    {
        var visibleInfo = CreateConfigurationInfo<VisibleConfig>(Guid.NewGuid(), "Visible", "Visible plugin");
        var hiddenInfo = CreateConfigurationInfo<HiddenConfig>(Guid.NewGuid(), "Hidden", "Hidden plugin");

        var configurationService = new Mock<IConfigurationService>();
        configurationService.Setup(service => service.GetAllConfigurationInfos()).Returns([visibleInfo, hiddenInfo]);

        var coordinator = new ConfigurationApiCoordinator(configurationService.Object, new Mock<IPluginManager>().Object);

        var result = coordinator.GetConfigurations(new ConfigurationDiscoveryFilter() { Hidden = IncludeOnlyFilter.False });

        Assert.Single(result);
        Assert.Equal(visibleInfo.ID, result[0].ID);
        Assert.Equal("Visible", result[0].Name);
    }

    [Fact]
    public void GetConfigurations_UsesPluginScope_WhenPluginIdIsSpecified()
    {
        var pluginId = Guid.NewGuid();
        var plugin = new TestPlugin(pluginId, "Scoped plugin");
        var scopedInfo = CreateConfigurationInfo<VisibleConfig>(Guid.NewGuid(), "Scoped", "Scoped plugin");

        var configurationService = new Mock<IConfigurationService>();
        configurationService.Setup(service => service.GetConfigurationInfo(plugin)).Returns([scopedInfo]);

        var pluginManager = new Mock<IPluginManager>();
        pluginManager.Setup(manager => manager.GetPluginInfo(pluginId)).Returns(CreatePluginInfo(pluginId, plugin, "Scoped plugin", isActive: true));

        var coordinator = new ConfigurationApiCoordinator(configurationService.Object, pluginManager.Object);

        var result = coordinator.GetConfigurations(new ConfigurationDiscoveryFilter() { PluginID = pluginId });

        Assert.Single(result);
        Assert.Equal(scopedInfo.ID, result[0].ID);
        configurationService.Verify(service => service.GetAllConfigurationInfos(), Times.Never);
        configurationService.Verify(service => service.GetConfigurationInfo(plugin), Times.Once);
    }

    [Fact]
    public void GetConfigurations_ReturnsEmpty_WhenPluginIsInactive()
    {
        var pluginId = Guid.NewGuid();

        var configurationService = new Mock<IConfigurationService>();
        var pluginManager = new Mock<IPluginManager>();
        pluginManager.Setup(manager => manager.GetPluginInfo(pluginId)).Returns(CreatePluginInfo(pluginId, null, "Inactive plugin", isActive: false));

        var coordinator = new ConfigurationApiCoordinator(configurationService.Object, pluginManager.Object);

        var result = coordinator.GetConfigurations(new ConfigurationDiscoveryFilter() { PluginID = pluginId });

        Assert.Empty(result);
        configurationService.Verify(service => service.GetAllConfigurationInfos(), Times.Never);
        configurationService.Verify(service => service.GetConfigurationInfo(It.IsAny<IPlugin>()), Times.Never);
    }

    [Fact]
    public void GetConfigurationInfo_ReturnsMetadataWithoutDocumentPayload()
    {
        var info = CreateConfigurationInfo<VisibleConfig>(Guid.NewGuid(), "Metadata", "Metadata plugin", hasCustomLoad: true);
        var configurationService = new Mock<IConfigurationService>();
        configurationService.Setup(service => service.GetConfigurationInfo(info.ID)).Returns(info);

        var coordinator = new ConfigurationApiCoordinator(configurationService.Object, new Mock<IPluginManager>().Object);

        var result = coordinator.GetConfigurationInfo(info.ID);

        Assert.NotNull(result);
        Assert.Equal(info.ID, result!.ID);
        Assert.Equal("Metadata", result.Name);
        Assert.True(result.HasCustomLoad);
        Assert.Equal(info.PluginInfo.ID, result.Plugin.ID);
    }

    [Fact]
    public void LoadDocument_UsesCustomLoadFlow_WhenConfigurationSupportsIt()
    {
        var info = CreateConfigurationInfo<VisibleConfig>(Guid.NewGuid(), "Custom Load", "Custom Load plugin", hasCustomLoad: true);

        var configuration = new VisibleConfig();
        var configurationService = new Mock<IConfigurationService>();
        configurationService.Setup(service => service.GetConfigurationInfo(info.ID)).Returns(info);
        configurationService.Setup(service => service.New(info)).Returns(configuration);
        configurationService.Setup(service => service.PerformReactiveAction(info, It.IsAny<IConfiguration>(), string.Empty, ConfigurationActionType.Load, ReactiveEventType.All, It.IsAny<IUser>(), It.IsAny<Uri?>()))
            .Returns(new ConfigurationActionResult(configuration));
        configurationService.Setup(service => service.Serialize(configuration)).Returns("{\"name\":\"custom\"}");

        var coordinator = new ConfigurationApiCoordinator(configurationService.Object, new Mock<IPluginManager>().Object);

        var result = coordinator.LoadDocument(info.ID, null, null);

        Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("{\"name\":\"custom\"}", result.Content);
    }

    [Fact]
    public void LoadDocument_ReturnsNotFound_ForUnknownConfiguration()
    {
        var configurationService = new Mock<IConfigurationService>();
        var coordinator = new ConfigurationApiCoordinator(configurationService.Object, new Mock<IPluginManager>().Object);

        var result = coordinator.LoadDocument(Guid.NewGuid(), null, null);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        Assert.Contains("not found", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetSchema_ReturnsSchemaForKnownConfiguration()
    {
        var info = CreateConfigurationInfo<VisibleConfig>(Guid.NewGuid(), "Schema", "Schema plugin");
        var configurationService = new Mock<IConfigurationService>();
        configurationService.Setup(service => service.GetConfigurationInfo(info.ID)).Returns(info);
        configurationService.Setup(service => service.GetSchema(info)).Returns("{\"type\":\"object\"}");

        var coordinator = new ConfigurationApiCoordinator(configurationService.Object, new Mock<IPluginManager>().Object);

        var result = coordinator.GetSchema(info.ID);

        Assert.Equal("{\"type\":\"object\"}", result);
    }

    [Fact]
    public void SaveDocument_ReturnsValidationFailure_WithoutSaving()
    {
        var info = CreateConfigurationInfo<VisibleConfig>(Guid.NewGuid(), "Save", "Save plugin");
        var configurationService = new Mock<IConfigurationService>();
        configurationService.Setup(service => service.GetConfigurationInfo(info.ID)).Returns(info);
        configurationService.Setup(service => service.Validate(info, "{\"name\":\"broken\"}"))
            .Returns(new Dictionary<string, IReadOnlyList<string>>() { ["name"] = ["Required"] });

        var coordinator = new ConfigurationApiCoordinator(configurationService.Object, new Mock<IPluginManager>().Object);

        var result = coordinator.SaveDocument(info.ID, "{\"name\":\"broken\"}", null, null);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.ValidationErrors);
        configurationService.Verify(service => service.Save(It.IsAny<Shoko.Abstractions.Config.ConfigurationInfo>(), It.IsAny<string>()), Times.Never);
    }

    private static Shoko.Abstractions.Config.ConfigurationInfo CreateConfigurationInfo<TConfig>(Guid id, string name, string pluginName, bool hasCustomLoad = false) where TConfig : class, IConfiguration, new()
    {
        var pluginInfo = CreatePluginInfo(Guid.NewGuid(), null, pluginName, isActive: false);
        return new(new Mock<IConfigurationService>().Object)
        {
            ID = id,
            Path = null,
            Name = name,
            Description = $"{name} description",
            HasCustomActions = false,
            HasCustomNewFactory = false,
            HasCustomValidation = false,
            HasCustomSave = false,
            HasCustomLoad = hasCustomLoad,
            HasLiveEdit = false,
            Type = typeof(TConfig),
            ContextualType = typeof(TConfig).ToContextualType(),
            Schema = new JsonSchema(),
            PluginInfo = pluginInfo,
        };
    }

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
            Types = plugin is null ? [typeof(VisibleConfig)] : [plugin.GetType()],
        };

    private class VisibleConfig : IConfiguration { }

    private class HiddenConfig : IConfiguration, IHiddenConfiguration { }

    private sealed class TestPlugin(Guid id, string name) : IPlugin
    {
        public Guid ID { get; } = id;

        public string Name { get; } = name;
    }
}
