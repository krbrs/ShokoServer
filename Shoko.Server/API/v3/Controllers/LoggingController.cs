using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoko.Abstractions.Logging.Services;
using Shoko.Abstractions.Web.Attributes;
using Shoko.Server.API.Annotations;
using Shoko.Server.API.v3.Models.Logging;
using Shoko.Server.Settings;

namespace Shoko.Server.API.v3.Controllers;

/// <summary>
/// Controller responsible for listing, reading, and downloading log files.
/// </summary>
[ApiController]
[Route("/api/v{version:apiVersion}/[controller]")]
[ApiV3]
[Authorize(Roles = "admin,init")]
[DatabaseBlockedExempt]
[InitFriendly]
public class LoggingController(ISettingsProvider settingsProvider, ILogService logService) : BaseController(settingsProvider)
{
    /// <summary>
    /// List all available log files.
    /// </summary>
    /// <returns>The available log files.</returns>
    [HttpGet("File")]
    public ActionResult<List<LogFile>> GetLogFiles()
        => logService.ListFiles().Select(file => new LogFile(file)).ToList();

    /// <summary>
    /// Read the current log file using line-based paging.
    /// </summary>
    /// <param name="offset">Line offset to start from.</param>
    /// <param name="limit">Maximum number of entries to return.</param>
    /// <returns>The paged log read result.</returns>
    [HttpGet("Current")]
    public ActionResult<LogReadResult> GetCurrentLogs(
        [FromQuery, Range(0, int.MaxValue)] int offset = 0,
        [FromQuery, Range(1, 1000)] int limit = 100
    )
        => new LogReadResult(logService.ReadCurrent(offset, limit));

    /// <summary>
    /// Read a specific log file using line-based paging.
    /// </summary>
    /// <param name="fileId">Log file identifier.</param>
    /// <param name="offset">Line offset to start from.</param>
    /// <param name="limit">Maximum number of entries to return.</param>
    /// <returns>The paged log read result.</returns>
    [HttpGet("File/{fileId}")]
    public ActionResult<LogReadResult> GetLogFileById(
        [FromRoute] string fileId,
        [FromQuery, Range(0, int.MaxValue)] int offset = 0,
        [FromQuery, Range(1, 1000)] int limit = 100
    )
    {
        try
        {
            return new LogReadResult(logService.ReadFile(fileId, offset, limit));
        }
        catch (FileNotFoundException)
        {
            return NotFound("Log file not found.");
        }
        catch (InvalidOperationException)
        {
            return BadRequest("Only JSONL logs support offset/limit reads.");
        }
    }

    /// <summary>
    /// Read log entries across files filtered by a date range.
    /// </summary>
    /// <param name="from">Optional start date (inclusive).</param>
    /// <param name="to">Optional end date (inclusive).</param>
    /// <param name="offset">Line offset to start from.</param>
    /// <param name="limit">Maximum number of entries to return.</param>
    /// <returns>The paged log read result.</returns>
    [HttpGet("Range")]
    public ActionResult<LogReadResult> GetLogRange(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery, Range(0, int.MaxValue)] int offset = 0,
        [FromQuery, Range(1, 1000)] int limit = 100
    )
    {
        if (from.HasValue && to.HasValue && from > to)
            return BadRequest("The 'from' query parameter must be before 'to'.");
        return new LogReadResult(logService.ReadRange(from?.ToUniversalTime(), to?.ToUniversalTime(), offset, limit));
    }

    /// <summary>
    /// Download a specific log file.
    /// </summary>
    /// <param name="fileId">Log file identifier.</param>
    /// <param name="decompress">Set to <c>true</c> to decompress supported compressed log files.</param>
    /// <returns>The file response.</returns>
    [HttpGet("File/{fileId}/Download")]
    [ProducesResponseType(typeof(FileStreamResult), 200)]
    public ActionResult DownloadLogFile([FromRoute] string fileId, [FromQuery] bool decompress = false)
    {
        try
        {
            var download = logService.OpenDownload(fileId, decompress);
            return File(download.Stream, download.ContentType, download.FileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound("Log file not found.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
