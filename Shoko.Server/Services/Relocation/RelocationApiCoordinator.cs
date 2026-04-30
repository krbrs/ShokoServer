using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
using ApiRelocationResult = Shoko.Server.API.v3.Models.Relocation.RelocationResult;
using Shoko.Server.API.v3.Models.Relocation.Input;
using Shoko.Server.Plugin;
using Shoko.Server.Models.Shoko;
using Shoko.Server.Services;
using Shoko.Server.Services.Configuration;
using Shoko.Server.Services.Relocation;
using Shoko.Server.Utilities;
using RelocationPipe = Shoko.Abstractions.Video.Relocation.RelocationPipe;

#nullable enable
namespace Shoko.Server.API.v3.Services;

public class RelocationApiCoordinator(IPluginManager pluginManager, IConfigurationService configurationService, IVideoService videoService, IVideoRelocationService relocationService)
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

    public RelocationBatchResult PreviewFiles(BatchRelocatePreviewBody body)
    {
        IRelocationPipe pipe;
        if (body is { ProviderID: null, Configuration: null or { Type: JTokenType.Null } })
        {
            if (relocationService.GetDefaultPipe() is not { ProviderInfo: { } } defaultPipe)
                return new() { StatusCode = HttpStatusCode.NotFound, Message = "Default RelocationPipe not available or otherwise unusable." };

            pipe = defaultPipe;
        }
        else
        {
            if (!body.ProviderID.HasValue)
                return new() { StatusCode = HttpStatusCode.BadRequest, Message = "The ProviderID must be provided if a configuration object is provided." };

            if (relocationService.GetProviderInfo(body.ProviderID.Value) is not { } providerInfo)
                return new() { StatusCode = HttpStatusCode.BadRequest, Message = "The RelocationProvider with the given ID was not found." };

            if (providerInfo.ConfigurationInfo is not null)
            {
                if (body.Configuration is null or { Type: JTokenType.Null })
                    return new() { StatusCode = HttpStatusCode.BadRequest, Message = "The RelocationProvider expects a configuration object, and the provided configuration is not a valid JSON object." };

                var data = body.Configuration.ToJson();
                var validationErrors = configurationService.Validate(providerInfo.ConfigurationInfo, data);
                if (validationErrors.Count > 0)
                    return new() { StatusCode = HttpStatusCode.BadRequest, ValidationErrors = validationErrors, Message = "Invalid configuration." };
                pipe = new RelocationPipe(providerInfo.ID, Encoding.UTF8.GetBytes(data));
            }
            else
            {
                if (body.Configuration is not null and not { Type: JTokenType.Null })
                    return new() { StatusCode = HttpStatusCode.BadRequest, Message = "The RelocationProvider does not expect a configuration object." };
                pipe = new RelocationPipe(providerInfo.ID, null);
            }
        }

        var results = new List<ApiRelocationResult>();
        foreach (var videoId in body.FileIDs)
        {
            if (videoService.GetVideoByID(videoId) is not VideoLocal video)
            {
                results.Add(new()
                {
                    FileID = videoId,
                    IsSuccess = false,
                    IsPreview = true,
                    ErrorMessage = $"Unable to find File with ID {videoId}",
                });
                continue;
            }

            var videoFile = video.FirstResolvedPlace ?? video.FirstValidPlace;
            if (videoFile is null)
            {
                results.Add(new()
                {
                    FileID = videoId,
                    IsSuccess = false,
                    IsPreview = true,
                    ErrorMessage = $"Unable to find any resolvable File.Location for File with ID {videoId}.",
                });
                continue;
            }

            RelocationResponse response;
            try
            {
                response = ((VideoRelocationService)relocationService).ProcessPipe(
                    videoFile,
                    pipe,
                    body.Move ?? relocationService.MoveOnImport,
                    body.Rename ?? relocationService.RenameOnImport,
                    body.AllowRelocationInsideDestination ?? relocationService.AllowRelocationInsideDestinationOnImport,
                    default
                );
            }
            catch (Exception ex)
            {
                response = RelocationResponse.FromError($"An error occurred while trying to find a new file location: {ex.Message}", ex);
            }

            results.Add(new()
            {
                FileID = videoId,
                FileLocationID = videoFile.ID,
                IsSuccess = response.Success,
                IsPreview = true,
                IsRelocated = response.Moved || response.Renamed,
                PipeName = pipe is IStoredRelocationPipe stored ? stored.Name : null,
                AbsolutePath = response.AbsolutePath,
                ManagedFolderID = response.ManagedFolder?.ID,
                RelativePath = response.RelativePath,
                ErrorMessage = response.Error?.Message,
            });
        }

        return new() { Results = results };
    }

    public async Task<RelocationBatchResult> RelocateFiles(BatchRelocateBody body)
    {
        if (relocationService.GetDefaultPipe() is not { ProviderInfo: { } } pipe)
            return new() { StatusCode = HttpStatusCode.NotFound, Message = "Default RelocationPipe not available or otherwise unusable." };

        var request = new AutoRelocateRequest
        {
            Pipe = pipe,
            DeleteEmptyDirectories = body.DeleteEmptyDirectories && (body.Move ?? relocationService.MoveOnImport),
            Move = body.Move ?? relocationService.MoveOnImport,
            Rename = body.Rename ?? relocationService.RenameOnImport,
            AllowRelocationInsideDestination = body.AllowRelocationInsideDestination ?? relocationService.AllowRelocationInsideDestinationOnImport,
        };

        var configName = request.Pipe is IStoredRelocationPipe stored ? stored.Name : null;
        var results = new List<ApiRelocationResult>();
        foreach (var fileId in body.FileIDs)
        {
            if (videoService.GetVideoByID(fileId) is not VideoLocal video)
            {
                results.Add(new()
                {
                    FileID = fileId,
                    IsSuccess = false,
                    PipeName = configName,
                    ErrorMessage = $"Unable to find File with ID {fileId}",
                });
                continue;
            }

            var place = video.FirstResolvedPlace ?? video.FirstValidPlace;
            if (place is null)
            {
                results.Add(new()
                {
                    FileID = fileId,
                    PipeName = configName,
                    IsSuccess = false,
                    ErrorMessage = $"Unable to find any resolvable File.Location for File with ID {fileId}.",
                });
                continue;
            }

            var result = await relocationService.AutoRelocateFile(place, request);
            if (!result.Success)
            {
                results.Add(new()
                {
                    FileID = fileId,
                    FileLocationID = place.ID,
                    PipeName = configName,
                    IsSuccess = false,
                    ErrorMessage = result.Error.Message,
                });
                continue;
            }

            results.Add(new()
            {
                FileID = fileId,
                FileLocationID = place.ID,
                ManagedFolderID = result.ManagedFolder.ID,
                PipeName = configName,
                IsSuccess = true,
                IsRelocated = true,
                RelativePath = result.RelativePath,
                AbsolutePath = result.AbsolutePath
            });
        }

        return new() { Results = results };
    }

    public RelocationBatchResult PreviewFiles(Guid pipeID, IEnumerable<int> fileIDs, bool? move = null, bool? rename = null, bool? allowRelocationInsideDestination = null)
    {
        if (relocationService.GetStoredPipe(pipeID) is not { } pipeInfo)
            return new() { StatusCode = HttpStatusCode.NotFound, Message = "Relocation pipe not found" };

        var results = new List<ApiRelocationResult>();
        foreach (var videoId in fileIDs)
        {
            if (videoService.GetVideoByID(videoId) is not VideoLocal video)
            {
                results.Add(new()
                {
                    FileID = videoId,
                    IsSuccess = false,
                    IsPreview = true,
                    ErrorMessage = $"Unable to find File with ID {videoId}",
                });
                continue;
            }

            var videoFile = video.FirstResolvedPlace ?? video.FirstValidPlace;
            if (videoFile is null)
            {
                results.Add(new()
                {
                    FileID = videoId,
                    IsSuccess = false,
                    IsPreview = true,
                    ErrorMessage = $"Unable to find any resolvable File.Location for File with ID {videoId}.",
                });
                continue;
            }

            RelocationResponse response;
            try
            {
                response = ((VideoRelocationService)relocationService).ProcessPipe(
                    videoFile,
                    pipeInfo,
                    move ?? relocationService.MoveOnImport,
                    rename ?? relocationService.RenameOnImport,
                    allowRelocationInsideDestination ?? relocationService.AllowRelocationInsideDestinationOnImport,
                    default
                );
            }
            catch (Exception ex)
            {
                response = RelocationResponse.FromError($"An error occurred while trying to find a new file location: {ex.Message}", ex);
            }

            results.Add(new()
            {
                FileID = videoId,
                FileLocationID = videoFile.ID,
                IsSuccess = response.Success,
                IsPreview = true,
                IsRelocated = response.Moved || response.Renamed,
                PipeName = pipeInfo is IStoredRelocationPipe stored ? stored.Name : null,
                AbsolutePath = response.AbsolutePath,
                ManagedFolderID = response.ManagedFolder?.ID,
                RelativePath = response.RelativePath,
                ErrorMessage = response.Error?.Message,
            });
        }

        return new() { Results = results };
    }

    public async Task<RelocationBatchResult> RelocateFiles(Guid pipeID, IEnumerable<int> fileIDs, bool deleteEmptyDirectories = true, bool? move = null, bool? rename = null, bool? allowRelocationInsideDestination = null)
    {
        if (relocationService.GetStoredPipe(pipeID) is not { } pipeInfo)
            return new() { StatusCode = HttpStatusCode.NotFound, Message = "Relocation pipe not found" };

        var request = new AutoRelocateRequest
        {
            Pipe = pipeInfo,
            DeleteEmptyDirectories = deleteEmptyDirectories && (move ?? relocationService.MoveOnImport),
            Move = move ?? relocationService.MoveOnImport,
            Rename = rename ?? relocationService.RenameOnImport,
            AllowRelocationInsideDestination = allowRelocationInsideDestination ?? relocationService.AllowRelocationInsideDestinationOnImport,
        };

        var configName = request.Pipe is IStoredRelocationPipe stored ? stored.Name : null;
        var results = new List<ApiRelocationResult>();
        foreach (var fileId in fileIDs)
        {
            if (videoService.GetVideoByID(fileId) is not VideoLocal video)
            {
                results.Add(new()
                {
                    FileID = fileId,
                    IsSuccess = false,
                    PipeName = configName,
                    ErrorMessage = $"Unable to find File with ID {fileId}",
                });
                continue;
            }

            var place = video.FirstResolvedPlace ?? video.FirstValidPlace;
            if (place is null)
            {
                results.Add(new()
                {
                    FileID = fileId,
                    PipeName = configName,
                    IsSuccess = false,
                    ErrorMessage = $"Unable to find any resolvable File.Location for File with ID {fileId}.",
                });
                continue;
            }

            var result = await relocationService.AutoRelocateFile(place, request);
            if (!result.Success)
            {
                results.Add(new()
                {
                    FileID = fileId,
                    FileLocationID = place.ID,
                    PipeName = configName,
                    IsSuccess = false,
                    ErrorMessage = result.Error.Message,
                });
                continue;
            }

            results.Add(new()
            {
                FileID = fileId,
                FileLocationID = place.ID,
                ManagedFolderID = result.ManagedFolder.ID,
                PipeName = configName,
                IsSuccess = true,
                IsRelocated = true,
                RelativePath = result.RelativePath,
                AbsolutePath = result.AbsolutePath
            });
        }

        return new() { Results = results };
    }
}
