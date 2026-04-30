using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Shoko.Abstractions.Config;
using Shoko.Abstractions.Config.Exceptions;
using Shoko.Abstractions.Config.Services;
using Shoko.Abstractions.Extensions;
using Shoko.Abstractions.Video.Relocation;
using Shoko.Abstractions.Video.Services;
using Shoko.Abstractions.Web.Attributes;
using Shoko.Server.API.Annotations;
using Shoko.Server.API.v3.Models.Relocation;
using Shoko.Server.API.v3.Models.Relocation.Input;
using Shoko.Server.Models.Shoko;
using Shoko.Server.Services;
using Shoko.Server.Services.Configuration;
using Shoko.Server.API.v3.Services;
using Shoko.Server.Services.Relocation;
using Shoko.Server.Settings;

using ApiRelocationPipe = Shoko.Server.API.v3.Models.Relocation.RelocationPipe;
using ApiRelocationResult = Shoko.Server.API.v3.Models.Relocation.RelocationResult;
using RelocationPipe = Shoko.Abstractions.Video.Relocation.RelocationPipe;

#nullable enable
namespace Shoko.Server.API.v3.Controllers;

/// <summary>
///   Controller responsible for handling file relocation. Interacts with the <see cref="IVideoRelocationService"/>.
/// </summary>
/// <param name="settingsProvider">
///   Settings provider.
/// </param>
/// <param name="configurationService">
///   Configuration Service.
/// </param>
    /// <param name="relocationService">
    ///   Relocation service.
    /// </param>
/// <param name="relocationApiCoordinator">
///   Relocation API coordinator.
/// </param>
[ApiController]
[Route("/api/v{version:apiVersion}/[controller]")]
[ApiV3]
[Authorize]
public class RelocationController(
    ISettingsProvider settingsProvider,
    IConfigurationService configurationService,
    IVideoRelocationService relocationService,
    RelocationApiCoordinator relocationApiCoordinator
) : BaseController(settingsProvider)
{
    #region Settings

    /// <summary>
    ///   Gets a summary of the relocation service's properties.
    /// </summary>
    /// <returns>
    ///   A <see cref="RelocationSummary"/> containing the current settings.
    /// </returns>
    [DatabaseBlockedExempt]
    [InitFriendly]
    [HttpGet("Summary")]
    public ActionResult<RelocationSummary> GetRelocationSummary()
        => new RelocationSummary
        {
            RenameOnImport = relocationService.RenameOnImport,
            MoveOnImport = relocationService.MoveOnImport,
            AllowRelocationInsideDestinationOnImport = relocationService.AllowRelocationInsideDestinationOnImport,
            ProviderCount = relocationService.GetAvailableProviders().Count(),
        };

    /// <summary>
    ///   Updates the relocation settings, such as the rename and move options.
    /// </summary>
    /// <param name="body">
    ///   The settings to update.
    /// </param>
    /// <returns>
    ///   An empty <see cref="ActionResult"/>.
    /// </returns>
    [Authorize(Roles = "admin,init")]
    [DatabaseBlockedExempt]
    [InitFriendly]
    [HttpPost("Settings")]
    public ActionResult UpdateRelocationSettings([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] UpdateRelocationSettingsBody body)
    {
        if (body.RenameOnImport.HasValue)
            relocationService.RenameOnImport = body.RenameOnImport.Value;
        if (body.MoveOnImport.HasValue)
            relocationService.MoveOnImport = body.MoveOnImport.Value;
        if (body.AllowRelocationInsideDestinationOnImport.HasValue)
            relocationService.AllowRelocationInsideDestinationOnImport = body.AllowRelocationInsideDestinationOnImport.Value;

        return Ok();
    }

    #endregion

    #region Providers

    /// <summary>
    ///   Gets all relocation providers available.
    /// </summary>
    /// <returns>
    ///   A list of all available <see cref="RelocationProvider"/>s.
    /// </returns>
    [DatabaseBlockedExempt]
    [InitFriendly]
    [HttpGet("Providers")]
    public ActionResult<List<RelocationProvider>> GetAvailableRelocationProviders([FromQuery] RelocationDiscoveryFilter filter)
        => relocationApiCoordinator.GetAvailableProviders(filter);

    [HttpGet("Provider")]
    [Obsolete("Use GET /api/v3/Relocation/Providers instead.")]
    public ActionResult<List<RelocationProvider>> GetAvailableRelocationProvidersLegacy([FromQuery] RelocationDiscoveryFilter filter)
        => GetAvailableRelocationProviders(filter);

    /// <summary>
    ///   Gets a specific relocation provider by ID.
    /// </summary>
    /// <param name="providerID">
    ///   The ID of the relocation provider to get.
    /// </param>
    /// <returns>
    ///   A <see cref="RelocationProvider"/>.
    /// </returns>
    [DatabaseBlockedExempt]
    [InitFriendly]
    [HttpGet("Provider/{providerID}")]
    public ActionResult<RelocationProvider> GetRelocationProviderByProviderID([FromRoute] Guid providerID)
        => relocationApiCoordinator.GetProviderByID(providerID) is { } value ? value : NotFound("Renamer not found");

    #endregion

    #region Preview

    /// <summary>
    ///   Preview running a relocation provider on a batch of files, using the
    ///   default pipe or the provided provider identified by ID and provided
    ///   configuration — if the provider identified by ID necessitates it.
    /// </summary>
    /// <param name="body">
    ///   The body, with the file IDs and optionally the provider ID and
    ///   configuration to use.
    /// </param>
    /// <returns>
    ///   A stream of relocate results.
    /// </returns>
    [Authorize("admin")]
    [HttpPost("Preview")]
    public ActionResult<IEnumerable<ApiRelocationResult>> BatchPreviewFilesWithProviderAndConfig(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] BatchRelocatePreviewBody body
    )
    {
        var result = relocationApiCoordinator.PreviewFiles(body);
        if (result.ValidationErrors is { Count: > 0 })
            return ValidationProblem(result.ValidationErrors);

        if (result.StatusCode is System.Net.HttpStatusCode.NotFound)
            return NotFound(result.Message);

        if (result.StatusCode is System.Net.HttpStatusCode.BadRequest)
            return BadRequest(result.Message);

        return Ok(result.Results ?? []);
    }

    #endregion

    #region Relocate

    /// <summary>
    ///   Run the default relocation pipe on a batch of files.
    /// </summary>
    /// <param name="body">
    ///   The body with the file IDs and relocation options to use.
    /// </param>
    /// <returns>
    ///   A stream of <see cref="ApiRelocationResult"/>s.
    /// </returns>
    [Authorize("admin")]
    [HttpPost("Relocate")]
    public async Task<ActionResult<IEnumerable<ApiRelocationResult>>> BatchRelocateFilesWithDefaultConfig(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] BatchRelocateBody body
    )
    {
        var result = await relocationApiCoordinator.RelocateFiles(body);
        if (result.ValidationErrors is { Count: > 0 })
            return ValidationProblem(result.ValidationErrors);

        if (result.StatusCode is System.Net.HttpStatusCode.NotFound)
            return NotFound(result.Message);

        if (result.StatusCode is System.Net.HttpStatusCode.BadRequest)
            return BadRequest(result.Message);

        return Ok(result.Results ?? []);
    }

    #endregion

    #region Pipes

    /// <summary>
    ///   Gets a list of all relocation pipes.
    /// </summary>
    /// <returns>
    ///   A list of <see cref="ApiRelocationPipe"/>s.
    /// </returns>
    [HttpGet("Pipe")]
    public ActionResult<List<ApiRelocationPipe>> GetAllRelocationPipes()
        => relocationService.GetStoredPipes()
            .Select(pipeInfo => new ApiRelocationPipe(pipeInfo, pipeInfo.ProviderInfo))
            .WhereNotNull()
            .ToList();

    /// <summary>
    ///   Create a new relocation pipe from the given body.
    /// </summary>
    /// <param name="body">
    ///   The details such as the name and provider of the pipe to be created,
    ///   optionally with the configuration if the provider requires it, but
    ///   it can be left out to use a new configuration.
    /// </param>
    /// <returns>
    ///   The newly created <see cref="ApiRelocationPipe"/>.
    /// </returns>
    [Authorize("admin")]
    [HttpPost("Pipe")]
    public ActionResult<ApiRelocationPipe> NewRelocationPipe([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] CreateRelocationPipeBody body)
    {
        if (string.IsNullOrWhiteSpace(body.Name)) return BadRequest("Name is required");
        if (relocationService.GetProviderInfo(body.ProviderID) is not { } providerInfo)
            return NotFound("The RelocationProvider with the given ID was not found.");

        IRelocationProviderConfiguration? configuration = null;
        if (providerInfo.ConfigurationInfo is not null)
        {
            if (body.Configuration is not null and not { Type: JTokenType.Null })
            {
                if (body.Configuration is not { Type: JTokenType.Object })
                    return ValidationProblem("The provided configuration is not a valid JSON object or null.", nameof(body.Configuration));

                try
                {
                    var data = body.Configuration.ToJson();
                    var validationProblems = configurationService.Validate(providerInfo.ConfigurationInfo, data);
                    if (validationProblems.Count > 0)
                        return ValidationProblem(validationProblems, nameof(body.Configuration));

                    configuration = (IRelocationProviderConfiguration)configurationService.Deserialize(providerInfo.ConfigurationInfo, data);
                }
                catch (Exception ex)
                {
                    return ValidationProblem(ex.Message, nameof(body.Configuration));
                }
            }
            else
            {
                configuration = (IRelocationProviderConfiguration)configurationService.New(providerInfo.ConfigurationInfo);
            }
        }
        var pipeInfo = relocationService.StorePipe(providerInfo.Provider, body.Name, configuration, body.IsDefault);

        return new ApiRelocationPipe(pipeInfo, pipeInfo.ProviderInfo);
    }

    /// <summary>
    ///   Get the relocation pipe by the given pipe ID.
    /// </summary>
    /// <param name="pipeID">
    ///   Relocation pipe ID.
    /// </param>
    /// <returns>
    ///   The <see cref="ApiRelocationPipe"/>.
    /// </returns>
    [HttpGet("Pipe/{pipeID}/Metadata")]
    public ActionResult<ApiRelocationPipe> GetRelocationPipeMetadataByPipeID([FromRoute] Guid pipeID)
    {
        if (relocationService.GetStoredPipe(pipeID) is not { } pipeInfo)
            return NotFound("Relocation pipe not found");

        return new ApiRelocationPipe(pipeInfo, pipeInfo.ProviderInfo);
    }

    [HttpGet("Pipe/{pipeID}")]
    [Obsolete("Use GET /api/v3/Relocation/Pipe/{pipeID}/Metadata instead.")]
    public ActionResult<ApiRelocationPipe> GetRelocationPipeByPipeID([FromRoute] Guid pipeID)
        => GetRelocationPipeMetadataByPipeID(pipeID);

    /// <summary>
    ///   Modify the relocation pipe by the given pipe ID.
    /// </summary>
    /// <param name="pipeID">
    ///    Relocation pipe ID.
    /// </param>
    /// <param name="body">
    ///   The details for what to update.
    /// </param>
    /// <returns></returns>
    [Authorize("admin")]
    [HttpPut("Pipe/{pipeID}")]
    public ActionResult<ApiRelocationPipe> PutRelocationPipeByPipeID([FromRoute] Guid pipeID, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] ModifyRelocationPipeBody body)
    {
        if (relocationService.GetStoredPipe(pipeID) is not { } pipeInfo)
            return NotFound("Relocation pipe not found");

        var updated = false;
        if (!string.IsNullOrEmpty(body.Name) && pipeInfo.Name != body.Name)
        {
            pipeInfo.Name = body.Name;
            updated = true;
        }
        if (body.IsDefault.HasValue && pipeInfo.IsDefault != body.IsDefault.Value)
        {
            pipeInfo.IsDefault = body.IsDefault.Value;
            updated = true;
        }
        if (updated)
            relocationService.UpdatePipe(pipeInfo);

        return new ApiRelocationPipe(pipeInfo, pipeInfo.ProviderInfo);
    }

    /// <summary>
    ///   Applies a JSON patch document to modify the relocation pipe by the
    ///   given pipe ID.
    /// </summary>
    /// <param name="pipeID">
    ///   Relocation pipe ID.
    /// </param>
    /// <param name="patchDocument">
    ///   A JSON Patch document containing the modifications to be applied to
    ///   the relocation pipe.
    /// </param>
    /// <returns>
    ///   The newly updated <see cref="ApiRelocationPipe"/>.
    /// </returns>
    [Authorize("admin")]
    [HttpPatch("Pipe/{pipeID}")]
    public ActionResult<ApiRelocationPipe> PatchRelocationPipeByPipeID([FromRoute] Guid pipeID, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] JsonPatchDocument<ModifyRelocationPipeBody> patchDocument)
    {
        if (relocationService.GetStoredPipe(pipeID) is not { } pipeInfo)
            return NotFound("Relocation pipe not found");

        var body = new ModifyRelocationPipeBody() { Name = pipeInfo.Name, IsDefault = pipeInfo.IsDefault };
        patchDocument.ApplyTo(body, ModelState);
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        return PutRelocationPipeByPipeID(pipeID, body);
    }

    /// <summary>
    /// Delete the relocation pipe by the given pipe ID.
    /// </summary>
    /// <param name="pipeID">
    ///   Relocation pipe ID.
    /// </param>
    /// <returns>
    ///   No content.
    /// </returns>
    [Authorize("admin")]
    [HttpDelete("Pipe/{pipeID}")]
    public ActionResult DeleteRelocationPipeByPipeID([FromRoute] Guid pipeID)
    {
        if (relocationService.GetStoredPipe(pipeID) is not { } pipeInfo)
            return NotFound("Relocation pipe not found");

        if (pipeInfo.IsDefault)
            return BadRequest("The default relocation pipe cannot be deleted.");

        relocationService.DeletePipe(pipeInfo);

        return NoContent();
    }

    /// <summary>
    ///   Get the relocation provider by the given pipe ID.
    /// </summary>
    /// <param name="pipeID">
    ///   Relocation pipe ID.
    /// </param>
    /// <returns>
    ///   The <see cref="RelocationProvider"/> for the pipe.
    /// </returns>
    [HttpGet("Pipe/{pipeID}/Provider")]
    public ActionResult<RelocationProvider> GetRelocationProviderByPipeID([FromRoute] Guid pipeID)
    {
        if (relocationService.GetStoredPipe(pipeID) is not { } pipeInfo)
            return NotFound("Relocation pipe not found");

        if (pipeInfo.ProviderInfo is not { } providerInfo)
            return NotFound("Relocation provider not found for relocation pipe.");

        return new RelocationProvider(providerInfo);
    }

    #region Pipes | Preview

    /// <summary>
    ///   Preview what would happen if you were to apply the relocation pipe by
    ///   the given pipe ID to the given files.
    /// </summary>
    /// <param name="pipeID">
    ///   Relocation pipe ID.
    /// </param>
    /// <param name="fileIDs">
    ///   The file IDs to preview.
    /// </param>
    /// <param name="move">
    ///   Whether or not to get the destination of the files. If <c>null</c>, to
    ///   <see cref="RelocationSummary.MoveOnImport"/>.
    /// </param>
    /// <param name="rename">
    ///   Whether or not to get the new name of the files. If <c>null</c>,
    ///   defaults to <see cref="RelocationSummary.RenameOnImport"/>.
    /// </param>
    /// <param name="allowRelocationInsideDestination">
    ///   Whether or not to allow relocation of files inside the destination. If
    ///   <c>null</c>, defaults to <see cref="RelocationSummary.AllowRelocationInsideDestinationOnImport"/>.
    /// </param>
    /// <returns>
    ///   A stream of <see cref="ApiRelocationResult"/>s.
    /// </returns>
    [Authorize("admin")]
    [HttpPost("Pipe/{pipeID}/Preview")]
    public ActionResult<IEnumerable<ApiRelocationResult>> BatchRelocateFilesByScriptID(
        [FromRoute] Guid pipeID,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] IEnumerable<int> fileIDs,
        bool? move = null,
        bool? rename = null,
        bool? allowRelocationInsideDestination = null
    )
        => ToBatchResult(relocationApiCoordinator.PreviewFiles(pipeID, fileIDs, move, rename, allowRelocationInsideDestination));

    #endregion

    #region Pipes | Relocate

    /// <summary>
    ///   Relocate the files with the relocation pipe by the given pipe ID.
    /// </summary>
    /// <param name="pipeID">
    ///   Relocation pipe ID.
    /// </param>
    /// <param name="fileIDs">
    ///   The file IDs to relocate.
    /// </param>
    /// <param name="deleteEmptyDirectories">
    ///   Whether or not to delete empty directories. Defaults to true.
    /// </param>
    /// <param name="move">
    ///   Whether or not to get the destination of the files. If <c>null</c>, to
    ///   <see cref="RelocationSummary.MoveOnImport"/>.
    /// </param>
    /// <param name="rename">
    ///   Whether or not to get the new name of the files. If <c>null</c>,
    ///   defaults to <see cref="RelocationSummary.RenameOnImport"/>.
    /// </param>
    /// <param name="allowRelocationInsideDestination">
    ///   Whether or not to allow relocation of files inside the destination. If
    ///   <c>null</c>, defaults to <see cref="RelocationSummary.AllowRelocationInsideDestinationOnImport"/>.
    /// </param>
    /// <returns>
    ///   A stream of <see cref="ApiRelocationResult"/>s.
    /// </returns>
    [Authorize("admin")]
    [HttpPost("Pipe/{pipeID}/Relocate")]
    public async Task<ActionResult<IAsyncEnumerable<ApiRelocationResult>>> BatchRelocateFilesByConfig(
        [FromRoute] Guid pipeID,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] IEnumerable<int> fileIDs,
        [FromQuery] bool deleteEmptyDirectories = true,
        [FromQuery] bool? move = null,
        [FromQuery] bool? rename = null,
        [FromQuery] bool? allowRelocationInsideDestination = null
    )
        => ToBatchResult(await relocationApiCoordinator.RelocateFiles(pipeID, fileIDs, deleteEmptyDirectories, move, rename, allowRelocationInsideDestination));

    #endregion

    #region Relocation | Configuration

    /// <summary>
    ///   Get the current configuration for the relocation pipe with the given
    ///   ID.
    /// </summary>
    /// <param name="pipeID">
    ///   Relocation pipe ID.
    /// </param>
    /// <returns>
    ///   The current configuration for the relocation pipe.
    /// </returns>
    [Produces("application/json")]
    [HttpGet("Pipe/{pipeID}/Document")]
    public ActionResult GetConfigurationForRelocationPipeByPipeID(Guid pipeID)
        => ToDocumentResult(relocationApiCoordinator.GetPipeConfiguration(pipeID));

    [HttpGet("Pipe/{pipeID}/Configuration")]
    [Obsolete("Use GET /api/v3/Relocation/Pipe/{pipeID}/Document instead.")]
    public ActionResult GetConfigurationForRelocationPipeByPipeIDLegacy(Guid pipeID)
        => GetConfigurationForRelocationPipeByPipeID(pipeID);

    /// <summary>
    ///   Overwrite the contents of the configuration for the relocation pipe
    ///   with the given ID.
    /// </summary>
    /// <param name="pipeID">
    ///   Relocation pipe ID.
    /// </param>
    /// <param name="body">
    ///   The new configuration.
    /// </param>
    /// <returns>
    ///   Ok if successful.
    /// </returns>
    [HttpPut("Pipe/{pipeID}/Document")]
    public ActionResult PutConfigurationForRelocationPipeByPipeID(Guid pipeID, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] JToken? body)
    {
        var json = body is null or { Type: JTokenType.Null } ? null : body.ToString(Newtonsoft.Json.Formatting.None, [new StringEnumConverter()]);
        return ToDocumentResult(relocationApiCoordinator.SavePipeConfiguration(pipeID, json));
    }

    [HttpPut("Pipe/{pipeID}/Configuration")]
    [Obsolete("Use PUT /api/v3/Relocation/Pipe/{pipeID}/Document instead.")]
    public ActionResult PutConfigurationForRelocationPipeByPipeIDLegacy(Guid pipeID, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] JToken? body)
        => PutConfigurationForRelocationPipeByPipeID(pipeID, body);

    /// <summary>
    ///   Patches the configuration for the relocation pipe with the given ID
    ///   using a JSON patch document.
    /// </summary>
    /// <param name="pipeID">
    ///   Relocation pipe ID.
    /// </param>
    /// <param name="patchDocument">
    ///   JSON patch document with operations to apply.
    /// </param>
    /// <returns>
    ///   Ok if successful.
    /// </returns>
    [HttpPatch("Pipe/{pipeID}/Document")]
    public ActionResult PatchConfigurationForRelocationPipeByPipeID(Guid pipeID, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] JsonPatchDocument patchDocument)
        => ToDocumentResult(relocationApiCoordinator.PatchPipeConfiguration(pipeID, patchDocument));

    [HttpPatch("Pipe/{pipeID}/Configuration")]
    [Obsolete("Use PATCH /api/v3/Relocation/Pipe/{pipeID}/Document instead.")]
    public ActionResult PatchConfigurationForRelocationPipeByPipeIDLegacy(Guid pipeID, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] JsonPatchDocument patchDocument)
        => PatchConfigurationForRelocationPipeByPipeID(pipeID, patchDocument);

    #endregion

    #endregion

    private ActionResult ToDocumentResult(RelocationPipeDocumentResult result)
    {
        if (result.ValidationErrors is { Count: > 0 })
            return ValidationProblem(result.ValidationErrors);

        return result.StatusCode switch
        {
            System.Net.HttpStatusCode.OK when result.Content is not null => Content(result.Content, result.ContentType),
            System.Net.HttpStatusCode.NoContent => NoContent(),
            System.Net.HttpStatusCode.NotFound => NotFound(result.Message),
            System.Net.HttpStatusCode.Conflict => Conflict(result.Message),
            _ when result.Content is not null => Content(result.Content, result.ContentType),
            _ when result.StatusCode is System.Net.HttpStatusCode.OK => Ok(),
            _ => StatusCode((int)result.StatusCode, result.Message)
        };
    }

    private ActionResult ToBatchResult(RelocationBatchResult result)
    {
        if (result.ValidationErrors is { Count: > 0 })
            return ValidationProblem(result.ValidationErrors);

        return result.StatusCode switch
        {
            System.Net.HttpStatusCode.NotFound => NotFound(result.Message),
            System.Net.HttpStatusCode.BadRequest => BadRequest(result.Message),
            System.Net.HttpStatusCode.Conflict => Conflict(result.Message),
            _ => Ok(result.Results ?? []),
        };
    }
}
