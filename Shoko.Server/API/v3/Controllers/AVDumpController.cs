
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Shoko.Server.API.Annotations;
using Shoko.Server.Providers.AniDB;
using Shoko.Server.Providers.AniDB.Interfaces;
using Shoko.Server.Providers.AniDB.UDP.Exceptions;
using Shoko.Server.Settings;

#nullable enable
namespace Shoko.Server.API.v3.Controllers;

/// <summary>
/// A controller to configure the AVDump component.
/// </summary>
[ApiController]
[Route("/api/v{version:apiVersion}/[controller]")]
[ApiV3]
[Authorize("admin")]
public class AVDumpController : BaseController
{
    public AVDumpController(ISettingsProvider settingsProvider) : base(settingsProvider) { }

    /// <summary>
    /// Get status about the AVDump component.
    /// </summary>
    /// <returns></returns>
    [HttpGet("Status")]
    public Dictionary<string, object> GetAVDumpVersionInfo()
    {
        return new()
        {
            { "Installed", AVDumpHelper.IsAVDumpInstalled },
            { "InstalledVersion", AVDumpHelper.InstalledAVDumpVersion },
            { "ExpectedVersion", AVDumpHelper.AVDumpVersion },
        };
    }

    /// <summary>
    /// Update the installed AVDump component on a system.
    /// </summary>
    /// <param name="force">Forcefully update the AVDump component regardless
    /// of the version previously installed, if any.</param>
    /// <returns></returns>
    [HttpPost("Update")]
    public ActionResult<bool> UpdateAVDump([FromQuery] bool force = false)
    {
        if (!force)
        {
            var expectedVersion = AVDumpHelper.AVDumpVersion;
            var installedVersion = AVDumpHelper.InstalledAVDumpVersion;
            if (string.Equals(expectedVersion, installedVersion))
                return false;
        }

        return AVDumpHelper.UpdateAVDump();
    }
}
