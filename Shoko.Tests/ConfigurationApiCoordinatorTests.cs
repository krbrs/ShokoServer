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

    private static Shoko.Abstractions.Config.ConfigurationInfo CreateConfigurationInfo<TConfig>(Guid id, string name, string pluginName, bool hasCustomLoad = false) where TConfig : class, IConfiguration, new()
    {
        var pluginInfo = new LocalPluginInfo()
        {
            ID = Guid.NewGuid(),
            Name = pluginName,
            Description = pluginName,
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
            IsActive = false,
            CanLoad = true,
            CanUninstall = true,
            Plugin = null,
            PluginType = typeof(TConfig),
            ServiceRegistrationType = null,
            ApplicationRegistrationType = null,
            ContainingDirectory = null,
            DLLs = [],
            Types = [typeof(TConfig)],
        };

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

    private class VisibleConfig : IConfiguration { }

    private class HiddenConfig : IConfiguration, IHiddenConfiguration { }
}
