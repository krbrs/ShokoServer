using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Shoko.Abstractions.Config.Enums;
using Shoko.Abstractions.Web.Attributes;
using Shoko.Server.API.Annotations;
using Shoko.Server.API.v3.Models.Common;
using Shoko.Server.API.v3.Models.Configuration;
using Shoko.Server.API.v3.Models.Configuration.Input;
using Shoko.Server.Settings;
using Shoko.Server.Services.Configuration;
using Shoko.Server.Utilities;

#nullable enable
namespace Shoko.Server.API.v3.Controllers;

/// <summary>
/// Controller responsible for managing configurations.
/// </summary>
[ApiController]
[Route("/api/v{version:apiVersion}/[controller]")]
[ApiV3]
[Authorize(Roles = "admin,init")]
[DatabaseBlockedExempt]
[InitFriendly]
public class ConfigurationController(ISettingsProvider settingsProvider, ConfigurationApiCoordinator configurationApiCoordinator) : BaseController(settingsProvider)
{
    private Uri? _baseUri;

    public Uri BaseUri => _baseUri ??= new UriBuilder(
        Request.Scheme,
        Request.Host.Host,
        Request.Host.Port ?? (Request.Scheme == "https" ? 443 : 80),
        Request.PathBase,
        null
    ).Uri;

    /// <summary>
    ///   Get a list with information about all registered configurations.
    /// </summary>
    /// <param name="filter">A compact set of filters used to discover configurations.</param>
    /// <returns>
    ///   A list of <see cref="ConfigurationMetadata"/> for the configurations
    ///   matching the query.
    /// </returns>
    [HttpGet]
    public ActionResult<List<ConfigurationMetadata>> GetConfigurations([FromQuery] ConfigurationDiscoveryFilter filter)
        => configurationApiCoordinator.GetConfigurations(filter);

    /// <summary>
    /// Get the current configuration with the given id.
    /// </summary>
    /// <param name="id">Configuration id</param>
    /// <returns></returns>
    [Produces("application/json")]
    [HttpGet("{id:guid}/Document")]
    public ActionResult GetConfigurationDocument(Guid id)
        => ToDocumentResult(configurationApiCoordinator.LoadDocument(id, User, BaseUri));

    [Produces("application/json")]
    [HttpGet("{id:guid}")]
    public ActionResult GetConfiguration(Guid id)
        => GetConfigurationDocument(id);

    /// <summary>
    /// Overwrite the contents of the configuration with the given id.
    /// </summary>
    /// <param name="id">Configuration id</param>
    /// <param name="body">Configuration data</param>
    /// <returns></returns>
    [HttpPut("{id:guid}/Document")]
    public ActionResult<ConfigurationActionResult> UpdateConfiguration(Guid id, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] JToken body)
    {
        var json = body.ToString(Newtonsoft.Json.Formatting.None, [new StringEnumConverter()]);
        return ToActionResult(configurationApiCoordinator.SaveDocument(id, json, User, BaseUri));
    }

    [HttpPut("{id:guid}")]
    [Obsolete("Use PUT /api/v3/Configuration/{id}/Document instead.")]
    public ActionResult<ConfigurationActionResult> UpdateConfigurationLegacy(Guid id, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] JToken body)
        => UpdateConfiguration(id, body);

    /// <summary>
    /// Patches the configuration with the given id using a JSON patch document.
    /// </summary>
    /// <param name="id">Configuration id</param>
    /// <param name="patchDocument">JSON patch document with operations to apply.</param>
    /// <returns></returns>
    [HttpPatch("{id:guid}/Document")]
    public ActionResult<ConfigurationActionResult> PartiallyUpdateConfiguration(Guid id, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] JsonPatchDocument patchDocument)
    {
        return ToActionResult(configurationApiCoordinator.PatchDocument(id, patchDocument, User, BaseUri));
    }

    [HttpPatch("{id:guid}")]
    [Obsolete("Use PATCH /api/v3/Configuration/{id}/Document instead.")]
    public ActionResult<ConfigurationActionResult> PartiallyUpdateConfigurationLegacy(Guid id, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] JsonPatchDocument patchDocument)
        => PartiallyUpdateConfiguration(id, patchDocument);

    /// <summary>
    /// Get the information about the current configuration with the given id.
    /// </summary>
    /// <param name="id">Configuration id</param>
    /// <returns></returns>
    [HttpGet("{id:guid}/Metadata")]
    public ActionResult<ConfigurationMetadata> GetConfigurationMetadata(Guid id)
        => configurationApiCoordinator.GetConfigurationInfo(id) is { } configInfo ? configInfo : NotFound($"Configuration '{id}' not found!");

    [HttpGet("{id:guid}/Info")]
    [Obsolete("Use GET /api/v3/Configuration/{id}/Metadata instead.")]
    public ActionResult<ConfigurationMetadata> GetConfigurationInfo(Guid id)
        => GetConfigurationMetadata(id);

    /// <summary>
    /// Get the schema for the current configuration with the given id.
    /// </summary>
    /// <param name="id">Configuration id</param>
    /// <returns></returns>
    [Produces("application/json")]
    [HttpGet("{id:guid}/Schema")]
    public ActionResult SchemaConfiguration(Guid id)
    {
        try
        {
            return Content(configurationApiCoordinator.GetSchema(id), "application/json");
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Configuration '{id}' not found!");
        }
    }

    /// <summary>
    /// Create a new configuration unused instance of the configuration with the given id.
    /// </summary>
    /// <param name="id">Configuration id</param>
    /// <returns></returns>
    [Produces("application/json")]
    [HttpPost("{id:guid}/Document/New")]
    public ActionResult NewConfigurationDocument(Guid id)
        => ToDocumentResult(configurationApiCoordinator.CreateNewDocument(id, User, BaseUri));

    [HttpPost("{id:guid}/New")]
    [Obsolete("Use POST /api/v3/Configuration/{id}/Document/New instead.")]
    public ActionResult NewConfiguration(Guid id)
        => NewConfigurationDocument(id);

    /// <summary>
    /// Validate the configuration with the given id.
    /// </summary>
    /// <param name="id">Configuration id</param>
    /// <param name="body">Configuration data</param>
    /// <returns></returns>
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [HttpPost("{id:guid}/Validate")]
    public ActionResult<ConfigurationActionResult> ValidateConfiguration(Guid id, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] JToken body)
    {
        var json = body.ToString(Newtonsoft.Json.Formatting.None, [new StringEnumConverter()]);
        return ToActionResult(configurationApiCoordinator.ValidateDocument(id, json, User, BaseUri));
    }

    /// <summary>
    /// Perform an action on the configuration with the given id.
    /// </summary>
    /// <param name="id">Configuration id</param>
    /// <param name="body">Optional. Configuration data to perform the action on.</param>
    /// <param name="actionName">Action to perform</param>
    /// <returns></returns>
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [HttpPost("{id:guid}/Actions/{actionName}")]
    public ActionResult<ConfigurationActionResult> PerformConfigurationAction(
        Guid id,
        [FromRoute] string actionName,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] ConfigurationActionRequest body)
        => ToActionResult(configurationApiCoordinator.PerformCustomAction(id, actionName, body.Path, body.Configuration, User, BaseUri));

    [HttpPost("{id:guid}/PerformAction")]
    [Obsolete("Use POST /api/v3/Configuration/{id}/Actions/{actionName} instead.")]
    public ActionResult<ConfigurationActionResult> PerformConfigurationActionLegacy(
        Guid id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] JToken? body,
        [FromQuery] string path = "",
        [FromQuery] string actionName = "")
        => PerformConfigurationAction(id, actionName, new() { Configuration = body, Path = path });

    /// <summary>
    /// Perform an action on the configuration with the given id.
    /// </summary>
    /// <param name="id">Configuration id</param>
    /// <param name="body">Optional. Configuration data to perform the action on.</param>
    /// <returns></returns>
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [HttpPost("{id:guid}/Actions/LiveEdit")]
    public ActionResult<ConfigurationActionResult> PerformConfigurationLiveEdit(
        Guid id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] ConfigurationLiveEditRequest body)
        => ToActionResult(configurationApiCoordinator.PerformLiveEdit(id, body.Path, body.ReactiveEventType, body.Configuration, User, BaseUri));

    [HttpPost("{id:guid}/LiveEdit")]
    [Obsolete("Use POST /api/v3/Configuration/{id}/Actions/LiveEdit instead.")]
    public ActionResult<ConfigurationActionResult> PerformConfigurationLiveEditLegacy(
        Guid id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] JToken body,
        [FromQuery] ReactiveEventType reactiveEventType = ReactiveEventType.All,
        [FromQuery] string path = "")
        => PerformConfigurationLiveEdit(id, new() { Configuration = body, Path = path, ReactiveEventType = reactiveEventType });

    private ActionResult ToDocumentResult(ConfigurationDocumentResult result)
    {
        if (result.ValidationErrors is { Count: > 0 })
            return ValidationProblem(result.ValidationErrors);

        return result.StatusCode switch
        {
            System.Net.HttpStatusCode.OK when result.Content is not null => Content(result.Content, result.ContentType),
            System.Net.HttpStatusCode.NoContent => NoContent(),
            System.Net.HttpStatusCode.Conflict => Conflict(result.Message),
            System.Net.HttpStatusCode.NotFound => NotFound(result.Message),
            _ when result.Content is not null => Content(result.Content, result.ContentType),
            _ => StatusCode((int)result.StatusCode, result.Message)
        };
    }

    private ActionResult<ConfigurationActionResult> ToActionResult(ConfigurationActionOutcome result)
    {
        if (result.ValidationErrors is { Count: > 0 })
            return ValidationProblem(result.ValidationErrors);

        if (result.StatusCode is System.Net.HttpStatusCode.NotFound)
            return NotFound(result.Message);

        if (result.StatusCode is System.Net.HttpStatusCode.Conflict)
            return Conflict(result.Message);

        if (result.StatusCode is System.Net.HttpStatusCode.BadRequest)
            return BadRequest(result.Message);

        return result.Result is not null ? Ok(result.Result) : Ok(new ConfigurationActionResult());
    }
}
