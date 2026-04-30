using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Shoko.Abstractions.Config.Enums;
using Shoko.Abstractions.Extensions;
using Shoko.Abstractions.Config.Services;
using Shoko.Abstractions.Plugin;
using Shoko.Abstractions.User;
using Shoko.Server.API.v3.Models.Common;
using Shoko.Server.API.v3.Models.Configuration;
using Shoko.Server.API.v3.Models.Configuration.Input;
using Shoko.Server.Plugin;
using Shoko.Server.Utilities;

using ConfigurationActionType = Shoko.Abstractions.Config.Enums.ConfigurationActionType;

#nullable enable
namespace Shoko.Server.Services.Configuration;

public class ConfigurationApiCoordinator(IConfigurationService configurationService, IPluginManager pluginManager)
{
    public List<ConfigurationInfo> GetConfigurations(ConfigurationDiscoveryFilter filter)
    {
        var enumerable = filter.PluginID.HasValue
            ? pluginManager.GetPluginInfo(filter.PluginID.Value) is { IsActive: true } pluginInfo
                ? configurationService.GetConfigurationInfo(pluginInfo.Plugin)
                : []
            : configurationService.GetAllConfigurationInfos();

        if (!string.IsNullOrEmpty(filter.Query))
            enumerable = enumerable
                .Search(filter.Query, c => [c.Name])
                .Select(c => c.Result)
                .OrderByDescending(p => typeof(CorePlugin) == p.PluginInfo.PluginType)
                .ThenBy(p => p.PluginInfo.Name)
                .ThenBy(p => p.Name)
                .ThenBy(p => p.ID);

        return enumerable
            .Where(configurationInfo =>
            {
                if (filter.Hidden is not IncludeOnlyFilter.True)
                {
                    var shouldHideHidden = filter.Hidden is IncludeOnlyFilter.False;
                    if (shouldHideHidden == configurationInfo.IsHidden)
                        return false;
                }

                if (filter.IsBase is not IncludeOnlyFilter.True)
                {
                    var shouldHideBase = filter.IsBase is IncludeOnlyFilter.False;
                    if (shouldHideBase == configurationInfo.IsBase)
                        return false;
                }

                if (filter.CustomNewFactory is not IncludeOnlyFilter.True)
                {
                    var shouldHideCustomNewFactory = filter.CustomNewFactory is IncludeOnlyFilter.False;
                    if (shouldHideCustomNewFactory == configurationInfo.HasCustomNewFactory)
                        return false;
                }

                if (filter.CustomValidation is not IncludeOnlyFilter.True)
                {
                    var shouldHideCustomValidation = filter.CustomValidation is IncludeOnlyFilter.False;
                    if (shouldHideCustomValidation == configurationInfo.HasCustomValidation)
                        return false;
                }

                if (filter.CustomActions is not IncludeOnlyFilter.True)
                {
                    var shouldHideCustomActions = filter.CustomActions is IncludeOnlyFilter.False;
                    if (shouldHideCustomActions == configurationInfo.HasCustomActions)
                        return false;
                }

                if (filter.CustomSave is not IncludeOnlyFilter.True)
                {
                    var shouldHideCustomSave = filter.CustomSave is IncludeOnlyFilter.False;
                    if (shouldHideCustomSave == configurationInfo.HasCustomSave)
                        return false;
                }

                if (filter.CustomLoad is not IncludeOnlyFilter.True)
                {
                    var shouldHideCustomLoad = filter.CustomLoad is IncludeOnlyFilter.False;
                    if (shouldHideCustomLoad == configurationInfo.HasCustomLoad)
                        return false;
                }

                if (filter.LiveEdit is not IncludeOnlyFilter.True)
                {
                    var shouldHideReactiveActions = filter.LiveEdit is IncludeOnlyFilter.False;
                    if (shouldHideReactiveActions == configurationInfo.HasLiveEdit)
                        return false;
                }

                return true;
            })
            .Select(configurationInfo => new ConfigurationInfo(configurationInfo))
            .ToList();
    }

    public ConfigurationDocumentResult LoadDocument(Guid id, IUser? user, Uri? baseUri)
    {
        if (configurationService.GetConfigurationInfo(id) is not { } configInfo)
            return new() { StatusCode = HttpStatusCode.NotFound, Message = $"Configuration '{id}' not found!" };

        try
        {
            if (configInfo.HasCustomLoad)
            {
                var tempConfig = configurationService.New(configInfo);
                var result = configurationService.PerformReactiveAction(configInfo, tempConfig, string.Empty, ConfigurationActionType.Load, default, user, baseUri);
                if (result.ValidationErrors is { Count: > 0 })
                    return new() { StatusCode = HttpStatusCode.BadRequest, ValidationErrors = result.ValidationErrors };
                if (result.Configuration is not null)
                    return new() { Content = configurationService.Serialize(result.Configuration) };
                return new() { StatusCode = HttpStatusCode.Conflict, Message = "Unable to load custom configuration object for the user." };
            }

            var config = configurationService.Load(configInfo);
            return new() { Content = configurationService.Serialize(config) };
        }
        catch (Shoko.Abstractions.Config.Exceptions.ConfigurationValidationException ex)
        {
            return new() { StatusCode = HttpStatusCode.BadRequest, ValidationErrors = ex.ValidationErrors };
        }
    }

    public ConfigurationDocumentResult CreateNewDocument(Guid id, IUser? user, Uri? baseUri)
    {
        if (configurationService.GetConfigurationInfo(id) is not { } configInfo)
            return new() { StatusCode = HttpStatusCode.NotFound, Message = $"Configuration '{id}' not found!" };

        var config = configurationService.New(configInfo);
        var json = configurationService.Serialize(config);
        if (configInfo.HasCustomNewFactory)
        {
            var result = configurationService.PerformReactiveAction(configInfo, config, string.Empty, ConfigurationActionType.New, default, user, baseUri);
            if (result.ValidationErrors is { Count: > 0 })
                return new() { StatusCode = HttpStatusCode.BadRequest, ValidationErrors = result.ValidationErrors };
            if (result.Configuration is not null)
                return new() { Content = configurationService.Serialize(result.Configuration) };
            return new() { StatusCode = HttpStatusCode.Conflict, Message = "Unable to create a new custom configuration object for the user." };
        }

        return new() { Content = json };
    }

    public ConfigurationActionOutcome SaveDocument(Guid id, string json, IUser? user, Uri? baseUri)
    {
        if (configurationService.GetConfigurationInfo(id) is not { } configInfo)
            return new() { StatusCode = HttpStatusCode.NotFound, Message = $"Configuration '{id}' not found!" };

        try
        {
            if (configInfo.HasCustomSave)
            {
                if (configurationService.Validate(configInfo, json) is { Count: > 0 } errors)
                    return new() { StatusCode = HttpStatusCode.BadRequest, ValidationErrors = errors };

                var config = configurationService.Deserialize(configInfo, json);
                var result = configurationService.PerformReactiveAction(configInfo, config, string.Empty, ConfigurationActionType.Save, default, user, baseUri);
                return new() { Result = new(result, configurationService, json) };
            }

            var modified = configurationService.Save(configInfo, json);
            return new() { Result = new() { ShowSaveMessage = modified, Refresh = modified } };
        }
        catch (Shoko.Abstractions.Config.Exceptions.ConfigurationValidationException ex)
        {
            return new() { StatusCode = HttpStatusCode.BadRequest, ValidationErrors = ex.ValidationErrors };
        }
    }

    public ConfigurationActionOutcome PatchDocument(Guid id, JsonPatchDocument patchDocument, IUser? user, Uri? baseUri)
    {
        if (configurationService.GetConfigurationInfo(id) is not { } configInfo)
            return new() { StatusCode = HttpStatusCode.NotFound, Message = $"Configuration '{id}' not found!" };

        try
        {
            var config = configurationService.Load(configInfo, copy: true);
            patchDocument.ApplyTo(config);

            if (configInfo.HasCustomSave)
            {
                var json = configurationService.Serialize(config);
                var result = configurationService.PerformReactiveAction(configInfo, config, string.Empty, ConfigurationActionType.Save, default, user, baseUri);
                return new() { Result = new(result, configurationService, json) };
            }

            var modified = configurationService.Save(configInfo, config);
            return new() { Result = new() { ShowSaveMessage = modified, Refresh = modified } };
        }
        catch (Shoko.Abstractions.Config.Exceptions.ConfigurationValidationException ex)
        {
            return new() { StatusCode = HttpStatusCode.BadRequest, ValidationErrors = ex.ValidationErrors };
        }
    }

    public ConfigurationActionOutcome ValidateDocument(Guid id, string json, IUser? user, Uri? baseUri)
    {
        if (configurationService.GetConfigurationInfo(id) is not { } configInfo)
            return new() { StatusCode = HttpStatusCode.NotFound, Message = $"Configuration '{id}' not found!" };

        try
        {
            var errors = configurationService.Validate(configInfo, json);
            if (errors.Count > 0)
                return new() { StatusCode = HttpStatusCode.BadRequest, ValidationErrors = errors };

            if (configInfo.HasCustomValidation)
            {
                var config = configurationService.Deserialize(configInfo, json);
                var result = configurationService.PerformReactiveAction(configInfo, config, string.Empty, ConfigurationActionType.Validate, default, user, baseUri);
                if (result.ValidationErrors is { Count: > 0 })
                    return new() { StatusCode = HttpStatusCode.BadRequest, ValidationErrors = result.ValidationErrors };
            }

            return new();
        }
        catch (Shoko.Abstractions.Config.Exceptions.ConfigurationValidationException ex)
        {
            return new() { StatusCode = HttpStatusCode.BadRequest, ValidationErrors = ex.ValidationErrors };
        }
    }

    public ConfigurationActionOutcome PerformCustomAction(Guid id, string actionName, string path, JToken? body, IUser? user, Uri? baseUri)
    {
        if (string.IsNullOrEmpty(actionName))
            return new() { StatusCode = HttpStatusCode.BadRequest, Message = "Missing 'actionName' parameter for custom action!" };

        if (configurationService.GetConfigurationInfo(id) is not { } configInfo)
            return new() { StatusCode = HttpStatusCode.NotFound, Message = $"Configuration '{id}' not found!" };

        try
        {
            var json = body?.ToString(Newtonsoft.Json.Formatting.None, [new StringEnumConverter()]);
            json ??= configurationService.Serialize(configurationService.Load(configInfo));
            var config = configurationService.Deserialize(configInfo, json);
            var result = configurationService.PerformCustomAction(configInfo, config, path, actionName, user, baseUri);
            return new() { Result = new(result, configurationService, json) };
        }
        catch (Shoko.Abstractions.Config.Exceptions.ConfigurationValidationException ex)
        {
            return new() { StatusCode = HttpStatusCode.BadRequest, ValidationErrors = ex.ValidationErrors };
        }
        catch (Shoko.Abstractions.Config.Exceptions.InvalidConfigurationActionException ex)
        {
            return new() { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
        }
    }

    public ConfigurationActionOutcome PerformLiveEdit(Guid id, string path, ReactiveEventType reactiveEventType, JToken body, IUser? user, Uri? baseUri)
    {
        if (configurationService.GetConfigurationInfo(id) is not { } configInfo)
            return new() { StatusCode = HttpStatusCode.NotFound, Message = $"Configuration '{id}' not found!" };

        try
        {
            var json = body?.ToString(Newtonsoft.Json.Formatting.None, [new StringEnumConverter()]) ?? "null";
            var config = configurationService.Deserialize(configInfo, json);
            if (config is null)
                return new() { StatusCode = HttpStatusCode.BadRequest, Message = "Unable to deserialize configuration!" };
            var result = configurationService.PerformReactiveAction(configInfo, config, path, ConfigurationActionType.LiveEdit, reactiveEventType, user, baseUri);
            return new() { Result = new(result, configurationService, json) };
        }
        catch (Shoko.Abstractions.Config.Exceptions.ConfigurationValidationException ex)
        {
            return new() { StatusCode = HttpStatusCode.BadRequest, ValidationErrors = ex.ValidationErrors };
        }
        catch (Shoko.Abstractions.Config.Exceptions.InvalidConfigurationActionException ex)
        {
            return new() { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
        }
    }

    public ConfigurationInfo? GetConfigurationInfo(Guid id)
        => configurationService.GetConfigurationInfo(id) is { } configInfo ? new(configInfo) : null;

    public string GetSchema(Guid id)
    {
        if (configurationService.GetConfigurationInfo(id) is not { } configInfo)
            throw new KeyNotFoundException($"Configuration '{id}' not found!");
        return configurationService.GetSchema(configInfo);
    }
}
