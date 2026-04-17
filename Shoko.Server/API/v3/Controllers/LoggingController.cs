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

#nullable enable
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
    /// Read log entries across files filtered by a date range.
    /// </summary>
    /// <param name="from">Optional start date (inclusive).</param>
    /// <param name="to">Optional end date (inclusive).</param>
    /// <param name="offset">Line offset to start from.</param>
    /// <param name="limit">Maximum number of entries to return.</param>
    /// <param name="descending">Set to <c>true</c> for newest-first ordering. Defaults to <c>true</c>.</param>
    /// <returns>The paged log read result.</returns>
    [HttpGet("Range/Read")]
    public ActionResult<LogReadResult> GetLogRange(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery, Range(0, int.MaxValue)] uint offset = 0,
        [FromQuery, Range(0, 1000)] uint limit = 100,
        [FromQuery] bool descending = true
    )
    {
        if (from.HasValue && to.HasValue && from > to)
            return BadRequest("The 'from' query parameter must be before 'to'.");
        return new LogReadResult(logService.ReadRange(from?.ToUniversalTime(), to?.ToUniversalTime(), offset, limit, descending));
    }
    /// <summary>
    /// Download log entries across files filtered by a date range.
    /// </summary>
    /// <param name="from">Optional start date (inclusive).</param>
    /// <param name="to">Optional end date (inclusive).</param>
    /// <param name="format">
    /// Format to return the log entries as. Can be <c>simple</c>, <c>full</c>,
    /// <c>json</c> or <c>legacy</c>.
    /// </param>
    /// <returns>The generated download response.</returns>
    [HttpGet("Range/Download")]
    public ActionResult DownloadLogRange(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] string? format = null
    )
    {
        if (from.HasValue && to.HasValue && from > to)
            (to, from) = (from, to);

        var download = logService.DownloadRange(from?.ToUniversalTime(), to?.ToUniversalTime(), format);
        return File(download.Stream, download.ContentType, download.FileName);
    }

    /// <summary>
    /// List all available log files.
    /// </summary>
    /// <returns>The available log files.</returns>
    [HttpGet("File")]
    public ActionResult<List<LogFile>> GetLogFiles()
        => logService.GetAllLogFiles().Select(file => new LogFile(file)).ToList();

    /// <summary>
    /// Get the current log file.
    /// </summary>
    /// <returns>The current log file.</returns>
    [HttpGet("File/Current")]
    public ActionResult<LogFile> GetCurrentLogFile()
        => new LogFile(logService.GetCurrentLogFile());

    /// <summary>
    /// Read the current log file using line-based paging.
    /// </summary>
    /// <param name="offset">Line offset to start from.</param>
    /// <param name="limit">Maximum number of entries to return.</param>
    /// <returns>The paged log read result.</returns>
    [HttpGet("File/Current/Read")]
    public ActionResult<LogReadResult> GetCurrentLogs(
        [FromQuery, Range(0, int.MaxValue)] uint offset = 0,
        [FromQuery, Range(0, 1000)] uint limit = 100
    )
    {
        var fileInfo = logService.GetCurrentLogFile();
        if (fileInfo.Format is not Abstractions.Logging.Models.LogFileFormat.JsonL)
            return BadRequest("Only JSONL logs support offset/limit reads.");

        return new LogReadResult(logService.ReadLogFile(fileInfo, offset, limit));
    }

    /// <summary>
    /// Download the current log file.
    /// </summary>
    /// <param name="format">
    /// Format to return the log entries as. Can be <c>simple</c>, <c>full</c>,
    /// <c>json</c> or <c>legacy</c>.
    /// </param>
    /// <returns>The current log file download response.</returns>
    [HttpGet("File/Current/Download")]
    [ProducesResponseType(typeof(FileStreamResult), 200)]
    public ActionResult DownloadCurrentLogFile(
        [FromQuery] string format = "simple"
    )
    {
        var fileInfo = logService.GetCurrentLogFile();
        var download = logService.DownloadLogFile(fileInfo, format);
        return File(download.Stream, download.ContentType, download.FileName);
    }

    /// <summary>
    /// Get a specific log file by identifier.
    /// </summary>
    /// <param name="fileId">Log file identifier.</param>
    /// <returns>The log file metadata.</returns>
    [HttpGet("File/{fileId}")]
    public ActionResult<LogFile> GetLogFileById([FromRoute] Guid fileId)
    {
        if (logService.GetLogFileByID(fileId) is not { } fileInfo)
            return NotFound("Log file not found.");

        return new LogFile(fileInfo);
    }

    /// <summary>
    /// Read a specific log file using line-based paging.
    /// </summary>
    /// <param name="fileId">Log file identifier.</param>
    /// <param name="offset">Line offset to start from.</param>
    /// <param name="limit">Maximum number of entries to return.</param>
    /// <returns>The paged log read result.</returns>
    [HttpGet("File/{fileId}/Read")]
    public ActionResult<LogReadResult> GetLogFileById(
        [FromRoute] Guid fileId,
        [FromQuery, Range(0, int.MaxValue)] uint offset = 0,
        [FromQuery, Range(0, 1000)] uint limit = 100
    )
    {
        if (logService.GetLogFileByID(fileId) is not { } fileInfo)
            return NotFound("Log file not found.");

        if (fileInfo.Format is not Abstractions.Logging.Models.LogFileFormat.JsonL)
            return BadRequest("Only JSONL logs support offset/limit reads.");

        return new LogReadResult(logService.ReadLogFile(fileInfo, offset, limit));
    }

    /// <summary>
    /// Download a specific log file.
    /// </summary>
    /// <param name="fileId">Log file identifier.</param>
    /// <param name="format">
    /// Format to return the log entries as. Can be <c>simple</c>, <c>full</c>,
    /// <c>json</c> or <c>legacy</c>.
    /// </param>
    /// <returns>The file response.</returns>
    [HttpGet("File/{fileId}/Download")]
    [ProducesResponseType(typeof(FileStreamResult), 200)]
    public ActionResult DownloadLogFile(
        [FromRoute] Guid fileId,
        [FromQuery] string format = "simple"
    )
    {
        if (logService.GetLogFileByID(fileId) is not { } fileInfo)
            return NotFound("Log file not found.");

        try
        {
            var download = logService.DownloadLogFile(fileInfo, format);
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

    /// <summary>
    /// Delete a specific log file.
    /// </summary>
    /// <param name="fileId">Log file identifier.</param>
    /// <returns>An empty success response when the file is deleted.</returns>
    [HttpDelete("File/{fileId}")]
    public ActionResult DeleteLogFile([FromRoute] Guid fileId)
    {
        if (logService.GetLogFileByID(fileId) is not { } fileInfo)
            return NotFound("Log file not found.");

        try
        {
            logService.DeleteLogFile(fileInfo);
            return Ok();
        }
        catch (InvalidOperationException)
        {
            return BadRequest("Unable to delete the current log file.");
        }
    }
}
