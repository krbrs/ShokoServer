using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Namotion.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NJsonSchema;
using Shoko.Abstractions.Config;
using Shoko.Abstractions.Config.Enums;
using Shoko.Abstractions.Config.Services;
using Shoko.Abstractions.Core;
using Shoko.Abstractions.Extensions;
using Shoko.Abstractions.Metadata.Enums;
using Shoko.Abstractions.Plugin;
using Shoko.Abstractions.Plugin.Models;
using Shoko.Abstractions.Video;
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

    [Fact]
    public void GetAvailableProviders_ReturnsEmpty_WhenPluginIsInactive()
    {
        var pluginId = Guid.NewGuid();

        var pluginManager = new Mock<IPluginManager>();
        pluginManager.Setup(manager => manager.GetPluginInfo(pluginId)).Returns(CreatePluginInfo(pluginId, null, "Inactive plugin", isActive: false));

        var relocationService = new Mock<IVideoRelocationService>();
        var coordinator = new RelocationApiCoordinator(pluginManager.Object, new Mock<IConfigurationService>().Object, new Mock<IVideoService>().Object, relocationService.Object);

        var result = coordinator.GetAvailableProviders(new RelocationDiscoveryFilter() { PluginID = pluginId });

        Assert.Empty(result);
        relocationService.Verify(service => service.GetAvailableProviders(), Times.Never);
        relocationService.Verify(service => service.GetProviderInfo(It.IsAny<IPlugin>()), Times.Never);
    }

    [Fact]
    public void GetPipeConfiguration_ReturnsStoredRawConfiguration_WhenProviderIsUnavailable()
    {
        var providerId = Guid.NewGuid();

        var relocationService = new Mock<IVideoRelocationService>();
        relocationService.Setup(service => service.GetProviderInfo(providerId)).Returns((RelocationProviderInfo?)null);
        var pipeInfo = new RelocationPipeInfo(relocationService.Object, new Mock<IConfigurationService>().Object, new FakeStoredPipe(providerId, "Stored pipe", "{\"name\":\"pipe\"}"));
        relocationService.Setup(service => service.GetStoredPipe(It.IsAny<Guid>())).Returns(pipeInfo);

        var coordinator = new RelocationApiCoordinator(new Mock<IPluginManager>().Object, new Mock<IConfigurationService>().Object, new Mock<IVideoService>().Object, relocationService.Object);

        var result = coordinator.GetPipeConfiguration(Guid.NewGuid());

        Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("{\"name\":\"pipe\"}", result.Content);
        Assert.Equal("application/json", result.ContentType);
    }

    [Fact]
    public void SavePipeConfiguration_ReturnsValidationFailure_WithoutSaving()
    {
        var providerId = Guid.NewGuid();
        var pipeId = Guid.NewGuid();

        var configurationService = new Mock<IConfigurationService>();
        configurationService.SetupGet(service => service.RestartPendingFor).Returns(new Dictionary<Guid, IReadOnlySet<string>>());
        configurationService.SetupGet(service => service.LoadedEnvironmentVariables).Returns(new Dictionary<Guid, IReadOnlySet<string>>());

        var providerInfo = CreateProviderInfoWithConfiguration(providerId, configurationService);
        var relocationService = new Mock<IVideoRelocationService>();
        relocationService.Setup(service => service.GetProviderInfo(providerId)).Returns(providerInfo);

        var pipeInfo = new RelocationPipeInfo(relocationService.Object, configurationService.Object, new FakeStoredPipe(providerId, "Stored pipe", "{\"name\":\"pipe\"}") { ID = pipeId });
        relocationService.Setup(service => service.GetStoredPipe(pipeId)).Returns(pipeInfo);

        configurationService.Setup(service => service.Validate(providerInfo.ConfigurationInfo!, "{\"name\":\"broken\"}"))
            .Returns(new Dictionary<string, IReadOnlyList<string>>() { ["name"] = ["Required"] });

        var coordinator = new RelocationApiCoordinator(new Mock<IPluginManager>().Object, configurationService.Object, new Mock<IVideoService>().Object, relocationService.Object);

        var result = coordinator.SavePipeConfiguration(pipeId, "{\"name\":\"broken\"}");

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.ValidationErrors);
        relocationService.Verify(service => service.UpdatePipe(It.IsAny<IStoredRelocationPipe>()), Times.Never);
    }

    [Fact]
    public void PatchPipeConfiguration_ReturnsOk_WhenPatchApplies()
    {
        var providerId = Guid.NewGuid();
        var pipeId = Guid.NewGuid();

        var configurationService = new Mock<IConfigurationService>();
        configurationService.SetupGet(service => service.RestartPendingFor).Returns(new Dictionary<Guid, IReadOnlySet<string>>());
        configurationService.SetupGet(service => service.LoadedEnvironmentVariables).Returns(new Dictionary<Guid, IReadOnlySet<string>>());

        var providerInfo = CreateProviderInfoWithConfiguration(providerId, configurationService);
        var relocationService = new Mock<IVideoRelocationService>();
        relocationService.Setup(service => service.GetProviderInfo(providerId)).Returns(providerInfo);

        var pipeInfo = new RelocationPipeInfo(relocationService.Object, configurationService.Object, new FakeStoredPipe(providerId, "Stored pipe", "{\"Name\":\"Stored\"}") { ID = pipeId });
        relocationService.Setup(service => service.GetStoredPipe(pipeId)).Returns(pipeInfo);

        configurationService.Setup(service => service.Validate(providerInfo.ConfigurationInfo!, It.IsAny<string>()))
            .Returns(new Dictionary<string, IReadOnlyList<string>>());

        var coordinator = new RelocationApiCoordinator(new Mock<IPluginManager>().Object, configurationService.Object, new Mock<IVideoService>().Object, relocationService.Object);

        var patch = new JsonPatchDocument();
        patch.Replace("/Name", "Updated pipe");

        var result = coordinator.PatchPipeConfiguration(pipeId, patch);

        Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public void PreviewFiles_ReturnsBadRequest_WhenProviderConfigurationIsInvalid()
    {
        var providerId = Guid.NewGuid();

        var configurationService = new Mock<IConfigurationService>();
        configurationService.SetupGet(service => service.RestartPendingFor).Returns(new Dictionary<Guid, IReadOnlySet<string>>());
        configurationService.SetupGet(service => service.LoadedEnvironmentVariables).Returns(new Dictionary<Guid, IReadOnlySet<string>>());

        var providerInfo = CreateProviderInfoWithConfiguration(providerId, configurationService);
        var relocationService = new Mock<IVideoRelocationService>();
        relocationService.Setup(service => service.GetProviderInfo(providerId)).Returns(providerInfo);

        configurationService.Setup(service => service.Validate(providerInfo.ConfigurationInfo!, "{\"name\":\"broken\"}"))
            .Returns(new Dictionary<string, IReadOnlyList<string>>() { ["name"] = ["Required"] });

        var coordinator = new RelocationApiCoordinator(new Mock<IPluginManager>().Object, configurationService.Object, new Mock<IVideoService>().Object, relocationService.Object);

        var result = coordinator.PreviewFiles(new BatchRelocatePreviewBody()
        {
            FileIDs = [123],
            ProviderID = providerId,
            Configuration = Newtonsoft.Json.Linq.JObject.Parse("{\"name\":\"broken\"}"),
        });

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.ValidationErrors);
    }

    [Fact]
    public void PreviewFiles_ReturnsBadRequest_WhenProviderDoesNotExpectConfigurationButConfigurationIsSupplied()
    {
        var providerId = Guid.NewGuid();

        var relocationService = new Mock<IVideoRelocationService>();
        relocationService.Setup(service => service.GetProviderInfo(providerId)).Returns(new RelocationProviderInfo()
        {
            ID = providerId,
            Version = new Version(1, 0, 0),
            Name = "Provider without config",
            Description = "Provider without config",
            Provider = Mock.Of<IRelocationProvider>(provider => provider.Name == "Provider without config" && provider.Description == "Provider without config"),
            ConfigurationInfo = null,
            PluginInfo = CreatePluginInfo(Guid.NewGuid(), null, "Provider without config plugin", isActive: true),
        });

        var coordinator = new RelocationApiCoordinator(new Mock<IPluginManager>().Object, new Mock<IConfigurationService>().Object, new Mock<IVideoService>().Object, relocationService.Object);

        var result = coordinator.PreviewFiles(new BatchRelocatePreviewBody()
        {
            FileIDs = [123],
            ProviderID = providerId,
            Configuration = Newtonsoft.Json.Linq.JObject.Parse("{\"name\":\"unexpected\"}"),
        });

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Contains("does not expect", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PreviewFiles_ReturnsNotFound_WhenDefaultPipeMissing()
    {
        var relocationService = new Mock<IVideoRelocationService>();
        var coordinator = new RelocationApiCoordinator(new Mock<IPluginManager>().Object, new Mock<IConfigurationService>().Object, new Mock<IVideoService>().Object, relocationService.Object);

        var result = coordinator.PreviewFiles(new BatchRelocatePreviewBody()
        {
            FileIDs = [123],
        });

        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        Assert.Contains("default", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RelocateFiles_ReturnsNotFound_WhenDefaultPipeMissing()
    {
        var relocationService = new Mock<IVideoRelocationService>();
        var coordinator = new RelocationApiCoordinator(new Mock<IPluginManager>().Object, new Mock<IConfigurationService>().Object, new Mock<IVideoService>().Object, relocationService.Object);

        var result = await coordinator.RelocateFiles(new BatchRelocateBody()
        {
            FileIDs = [123],
        });

        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        Assert.Contains("default", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RelocateFiles_ReturnsNotFound_WhenDefaultPipeProviderIsMissing()
    {
        var providerId = Guid.NewGuid();
        var relocationService = new Mock<IVideoRelocationService>();
        relocationService.Setup(service => service.GetDefaultPipe()).Returns(new RelocationPipeInfo(relocationService.Object, new Mock<IConfigurationService>().Object, new FakeStoredPipe(providerId, "Stored pipe", "{\"Name\":\"Stored\"}")));

        var coordinator = new RelocationApiCoordinator(new Mock<IPluginManager>().Object, new Mock<IConfigurationService>().Object, new Mock<IVideoService>().Object, relocationService.Object);

        var result = await coordinator.RelocateFiles(new BatchRelocateBody()
        {
            FileIDs = [123],
        });

        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        Assert.Contains("default", result.Message, StringComparison.OrdinalIgnoreCase);
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

    private static Shoko.Abstractions.Config.ConfigurationInfo CreateConfigurationInfo(Guid id, string name, Mock<IConfigurationService> configurationService)
    {
        configurationService.SetupGet(service => service.RestartPendingFor).Returns(new Dictionary<Guid, IReadOnlySet<string>>());
        configurationService.SetupGet(service => service.LoadedEnvironmentVariables).Returns(new Dictionary<Guid, IReadOnlySet<string>>());

        return new(configurationService.Object)
        {
            ID = id,
            Path = null,
            Name = name,
            Description = name,
            HasCustomActions = false,
            HasCustomNewFactory = false,
            HasCustomValidation = false,
            HasCustomSave = true,
            HasCustomLoad = false,
            HasLiveEdit = false,
            Type = typeof(FakeRelocationConfig),
            ContextualType = typeof(FakeRelocationConfig).ToContextualType(),
            Schema = new JsonSchema(),
            PluginInfo = CreatePluginInfo(Guid.NewGuid(), null, name, isActive: true),
        };
    }

    private static RelocationProviderInfo CreateProviderInfoWithConfiguration(Guid providerId, Mock<IConfigurationService> configurationService)
        => new()
        {
            ID = Guid.NewGuid(),
            Version = new Version(1, 0, 0),
            Name = "Provider with config",
            Description = "Provider with config",
            Provider = Mock.Of<IRelocationProvider>(provider =>
                provider.Name == "Provider with config" &&
                provider.Description == "Provider with config"),
            ConfigurationInfo = CreateConfigurationInfo(providerId, "RelocationConfig", configurationService),
            PluginInfo = CreatePluginInfo(Guid.NewGuid(), null, "Provider with config plugin", isActive: true),
        };

    private sealed class TestPlugin(Guid id, string name) : IPlugin
    {
        public Guid ID { get; } = id;

        public string Name { get; } = name;
    }

    private sealed class FakeRelocationConfig : IConfiguration
    {
        public string? Name { get; set; }
    }

    private sealed class FakeStoredPipe(Guid providerId, string name, string configuration) : IStoredRelocationPipe
    {
        public Guid ID { get; init; } = Guid.NewGuid();

        public string Name { get; } = name;

        public bool IsDefault { get; } = false;

        public Guid ProviderID { get; } = providerId;

        public byte[]? Configuration { get; } = Encoding.UTF8.GetBytes(configuration);
    }
}
