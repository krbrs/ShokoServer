using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Shoko.Abstractions.Config.Exceptions;
using Shoko.Abstractions.Config.Services;
using Shoko.Abstractions.Extensions;
using Shoko.Abstractions.Plugin;
using Shoko.Abstractions.Video.Relocation;
using Shoko.Abstractions.Video.Services;
using Shoko.Server.API.v3.Models.Relocation;
using Shoko.Server.API.v3.Models.Relocation.Input;
using Shoko.Server.Plugin;
using Shoko.Server.Utilities;

#nullable enable
namespace Shoko.Server.Services.Relocation;

public class RelocationApiCoordinator(IPluginManager pluginManager, IConfigurationService configurationService, IVideoRelocationService relocationService)
{
    public List<RelocationProvider> GetAvailableProviders(RelocationDiscoveryFilter filter)
    {
        var providers = filter.PluginID.HasValue
            ? pluginManager.GetPluginInfo(filter.PluginID.Value) is { IsActive: true } pluginInfo
                ? relocationService.GetProviderInfo(pluginInfo.Plugin)
                : []
            : relocationService.GetAvailableProviders();

        if (!string.IsNullOrEmpty(filter.Query))
        {
            providers = providers
                .Search(filter.Query, provider => [provider.Name, provider.Description])
                .Select(result => result.Result)
                .OrderByDescending(provider => typeof(CorePlugin) == provider.PluginInfo.PluginType)
                .ThenBy(provider => provider.PluginInfo.Name)
                .ThenBy(provider => provider.Name)
                .ThenBy(provider => provider.ID);
        }

        return providers.Select(providerInfo => new RelocationProvider(providerInfo)).ToList();
    }

    public RelocationProvider? GetProviderByID(Guid providerID)
        => relocationService.GetProviderInfo(providerID) is { } providerInfo ? new RelocationProvider(providerInfo) : null;

    public RelocationPipeDocumentResult GetPipeConfiguration(Guid pipeID)
    {
        if (relocationService.GetStoredPipe(pipeID) is not { } pipeInfo)
            return new() { StatusCode = HttpStatusCode.NotFound, Message = "Relocation pipe not found" };

        if (pipeInfo.ProviderInfo is not { } providerInfo)
        {
            if (pipeInfo.Configuration is null)
                return new() { StatusCode = HttpStatusCode.NotFound, Message = "Relocation provider not found for relocation pipe." };

            return new() { Content = Encoding.UTF8.GetString(pipeInfo.Configuration), ContentType = "application/json" };
        }

        if (providerInfo.ConfigurationInfo is null)
            return new() { StatusCode = HttpStatusCode.NotFound, Message = "Relocation provider does not support configuration." };

        try
        {
            var config = pipeInfo.LoadConfiguration();
            return new() { Content = configurationService.Serialize(config), ContentType = "application/json" };
        }
        catch (ConfigurationValidationException ex)
        {
            return new() { StatusCode = HttpStatusCode.BadRequest, ValidationErrors = ex.ValidationErrors };
        }
    }

    public RelocationPipeDocumentResult SavePipeConfiguration(Guid pipeID, string? json)
    {
        if (relocationService.GetStoredPipe(pipeID) is not { } pipeInfo)
            return new() { StatusCode = HttpStatusCode.NotFound, Message = "Relocation pipe not found" };

        try
        {
            var modified = pipeInfo.SaveConfiguration(json);
            return new() { StatusCode = modified ? HttpStatusCode.OK : HttpStatusCode.OK };
        }
        catch (ConfigurationValidationException ex)
        {
            return new() { StatusCode = HttpStatusCode.BadRequest, ValidationErrors = ex.ValidationErrors };
        }
    }

    public RelocationPipeDocumentResult PatchPipeConfiguration(Guid pipeID, JsonPatchDocument patchDocument)
    {
        if (relocationService.GetStoredPipe(pipeID) is not { } pipeInfo)
            return new() { StatusCode = HttpStatusCode.NotFound, Message = "Relocation pipe not found" };

        try
        {
            var config = pipeInfo.LoadConfiguration();
            patchDocument.ApplyTo(config);
            pipeInfo.SaveConfiguration(config);
            return new() { StatusCode = HttpStatusCode.OK };
        }
        catch (ConfigurationValidationException ex)
        {
            return new() { StatusCode = HttpStatusCode.BadRequest, ValidationErrors = ex.ValidationErrors };
        }
    }
}
