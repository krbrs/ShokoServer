using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Namotion.Reflection;
using Moq;
using Shoko.Abstractions.Config;
using Shoko.Abstractions.Extensions;
using Shoko.Abstractions.Config.Services;
using Shoko.Abstractions.Core;
using Shoko.Abstractions.Plugin;
using Shoko.Abstractions.Plugin.Models;
using Shoko.Abstractions.Video.Relocation;
using Shoko.Abstractions.Video.Services;
using Shoko.Server.API.v3.Controllers;
using ApiRelocationPipe = Shoko.Server.API.v3.Models.Relocation.RelocationPipe;
using Shoko.Server.API.v3.Models.Relocation;
using Shoko.Server.API.v3.Models.Relocation.Input;
using Shoko.Server.API.v3.Services;
using Shoko.Server.Services.Configuration;
using Shoko.Server.Services.Relocation;
using Shoko.Server.Settings;
using Xunit;

#nullable enable
namespace Shoko.Tests;

public class RelocationControllerTests
{
    [Fact]
    public void LegacyProviderListRoute_ReturnsSameResultAsPrimaryRoute()
    {
        var controller = CreateController(out var relocationService, out var configurationService, out var coordinator);
        var filter = new RelocationDiscoveryFilter();
        var providerInfo = CreateProviderInfo("Scoped provider");
        relocationService.Setup(service => service.GetAvailableProviders()).Returns([providerInfo]);

        var primary = controller.GetAvailableRelocationProviders(filter);
#pragma warning disable CS0618
        var legacy = controller.GetAvailableRelocationProvidersLegacy(filter);
#pragma warning restore CS0618

        Assert.Equal(primary.Value!.Count, legacy.Value!.Count);
        Assert.Equal(primary.Value![0].ID, legacy.Value![0].ID);
    }

    [Fact]
    public void LegacyPipeMetadataRoute_ReturnsSameResultAsPrimaryRoute()
    {
        var controller = CreateController(out var relocationService, out var configurationService, out var coordinator);
        var pipeId = Guid.NewGuid();
        var pipeInfo = CreatePipeInfo(relocationService.Object, configurationService.Object, pipeId, "Pipe", Guid.NewGuid(), "{\"name\":\"pipe\"}");
        relocationService.Setup(service => service.GetStoredPipe(pipeId)).Returns(pipeInfo);

        var primary = controller.GetRelocationPipeMetadataByPipeID(pipeId);
#pragma warning disable CS0618
        var legacy = controller.GetRelocationPipeByPipeID(pipeId);
#pragma warning restore CS0618

        Assert.Equal(((ApiRelocationPipe)primary.Value!).ID, ((ApiRelocationPipe)legacy.Value!).ID);
    }

    [Fact]
    public void LegacyDocumentRoute_ReturnsSameResultAsPrimaryRoute()
    {
        var controller = CreateController(out var relocationService, out var configurationService, out var coordinator);
        var pipeId = Guid.NewGuid();
        var pipeInfo = CreatePipeInfo(relocationService.Object, configurationService.Object, pipeId, "Pipe", Guid.NewGuid(), "{\"name\":\"pipe\"}");
        relocationService.Setup(service => service.GetStoredPipe(pipeId)).Returns(pipeInfo);

        var primary = controller.GetConfigurationForRelocationPipeByPipeID(pipeId);
#pragma warning disable CS0618
        var legacy = controller.GetConfigurationForRelocationPipeByPipeIDLegacy(pipeId);
#pragma warning restore CS0618

        var primaryContent = Assert.IsType<ContentResult>(primary);
        var legacyContent = Assert.IsType<ContentResult>(legacy);
        Assert.Equal(primaryContent.Content, legacyContent.Content);
        Assert.Equal(primaryContent.ContentType, legacyContent.ContentType);
    }

    [Fact]
    public void LegacyPatchAndPutRoutes_ReturnSameStatusAsPrimaryRoutes()
    {
        var controller = CreateController(out var relocationService, out var configurationService, out var coordinator);
        var pipeId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        var providerInfo = CreateProviderInfo("Pipe provider", providerId, CreateConfigurationInfo(providerId, configurationService));

        configurationService.Setup(service => service.Validate(providerInfo.ConfigurationInfo!, It.IsAny<string>()))
            .Returns(new Dictionary<string, IReadOnlyList<string>>());
        configurationService.Setup(service => service.Validate(providerInfo.ConfigurationInfo!, It.IsAny<IConfiguration>()))
            .Returns(new Dictionary<string, IReadOnlyList<string>>());
        configurationService.Setup(service => service.Deserialize(providerInfo.ConfigurationInfo!, It.IsAny<string>()))
            .Returns((ConfigurationInfo _, string json) => new FakeRelocationConfig() { Name = json.Contains("Updated", StringComparison.OrdinalIgnoreCase) ? "Updated" : "Stored" });
        configurationService.Setup(service => service.Serialize(It.IsAny<IConfiguration>()))
            .Returns((IConfiguration config) => $"{{\"Name\":\"{((FakeRelocationConfig)config).Name}\"}}");

        relocationService.Setup(service => service.GetProviderInfo(providerId)).Returns(providerInfo);
        var pipeInfo = CreatePipeInfo(relocationService.Object, configurationService.Object, pipeId, "Pipe", providerId, "{\"Name\":\"Stored\"}");
        relocationService.Setup(service => service.GetStoredPipe(pipeId)).Returns(pipeInfo);

        var putPrimary = controller.PutConfigurationForRelocationPipeByPipeID(pipeId, Newtonsoft.Json.Linq.JObject.Parse("{\"Name\":\"Updated\"}"));
#pragma warning disable CS0618
        var putLegacy = controller.PutConfigurationForRelocationPipeByPipeIDLegacy(pipeId, Newtonsoft.Json.Linq.JObject.Parse("{\"Name\":\"Updated\"}"));
        var patchPrimary = controller.PatchConfigurationForRelocationPipeByPipeID(pipeId, CreateNamePatch());
        var patchLegacy = controller.PatchConfigurationForRelocationPipeByPipeIDLegacy(pipeId, CreateNamePatch());
#pragma warning restore CS0618

        Assert.IsType<OkResult>(putPrimary);
        Assert.IsType<OkResult>(putLegacy);
        Assert.IsType<OkResult>(patchPrimary);
        Assert.IsType<OkResult>(patchLegacy);
    }

    private static RelocationController CreateController(out Mock<IVideoRelocationService> relocationService, out Mock<IConfigurationService> configurationService, out RelocationApiCoordinator coordinator)
    {
        var settingsProvider = Mock.Of<ISettingsProvider>();
        configurationService = new Mock<IConfigurationService>();
        configurationService.SetupGet(service => service.RestartPendingFor).Returns(new Dictionary<Guid, IReadOnlySet<string>>());
        configurationService.SetupGet(service => service.LoadedEnvironmentVariables).Returns(new Dictionary<Guid, IReadOnlySet<string>>());
        relocationService = new Mock<IVideoRelocationService>();
        coordinator = new RelocationApiCoordinator(
            Mock.Of<IPluginManager>(),
            configurationService.Object,
            Mock.Of<Shoko.Abstractions.Video.Services.IVideoService>(),
            relocationService.Object);

        return new RelocationController(settingsProvider, configurationService.Object, relocationService.Object, coordinator);
    }

    private static RelocationProviderInfo CreateProviderInfo(string name, Guid? providerId = null, Shoko.Abstractions.Config.ConfigurationInfo? configurationInfo = null)
        => new()
        {
            ID = providerId ?? Guid.NewGuid(),
            Version = new Version(1, 0, 0),
            Name = name,
            Description = name,
            Provider = Mock.Of<IRelocationProvider>(provider => provider.Name == name && provider.Description == name),
            ConfigurationInfo = configurationInfo,
            PluginInfo = CreatePluginInfo(Guid.NewGuid(), null, name, isActive: true),
        };

    private static Shoko.Abstractions.Config.ConfigurationInfo CreateConfigurationInfo(Guid id, Mock<IConfigurationService> configurationService)
    {
        configurationService.SetupGet(service => service.RestartPendingFor).Returns(new Dictionary<Guid, IReadOnlySet<string>>());
        configurationService.SetupGet(service => service.LoadedEnvironmentVariables).Returns(new Dictionary<Guid, IReadOnlySet<string>>());

        return new(configurationService.Object)
        {
            ID = id,
            Path = null,
            Name = "RelocationConfig",
            Description = "RelocationConfig",
            HasCustomActions = false,
            HasCustomNewFactory = false,
            HasCustomValidation = false,
            HasCustomSave = true,
            HasCustomLoad = false,
            HasLiveEdit = false,
            Type = typeof(FakeRelocationConfig),
            ContextualType = typeof(FakeRelocationConfig).ToContextualType(),
            Schema = new NJsonSchema.JsonSchema(),
            PluginInfo = CreatePluginInfo(Guid.NewGuid(), null, "RelocationConfig plugin", isActive: true),
        };
    }

    private static RelocationPipeInfo CreatePipeInfo(
        IVideoRelocationService relocationService,
        IConfigurationService configurationService,
        Guid id,
        string name,
        Guid providerId,
        string? configuration = null)
        => new(relocationService, configurationService, new FakeStoredPipe(providerId, name, configuration ?? string.Empty) { ID = id });

    private static Shoko.Abstractions.Plugin.Models.LocalPluginInfo CreatePluginInfo(Guid id, IPlugin? plugin, string name, bool isActive)
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

    private static JsonPatchDocument CreateNamePatch()
    {
        var patch = new JsonPatchDocument();
        patch.Replace("/Name", "Updated");
        return patch;
    }

    private sealed class FakeStoredPipe(Guid providerId, string name, string configuration) : IStoredRelocationPipe
    {
        public Guid ID { get; init; }

        public string Name { get; } = name;

        public bool IsDefault { get; } = false;

        public Guid ProviderID { get; } = providerId;

        public byte[]? Configuration { get; } = System.Text.Encoding.UTF8.GetBytes(configuration);
    }

    private sealed class FakeRelocationConfig : IConfiguration
    {
        public string? Name { get; set; }
    }
}
