using System;
using System.Collections.Generic;
using Shoko.Abstractions.Logging.Models;

namespace Shoko.Abstractions.Logging.Services;

/// <summary>
///   Service responsible for listing, reading, downloading, and maintaining
///   server log files.
/// </summary>
public interface ILogService
{
    /// <summary>
    ///   Gets the full path to the currently active log file.
    /// </summary>
    /// <returns>
    ///   The full path, or an empty string if no active file target is found.
    /// </returns>
    string GetCurrentLogFilePath();

    /// <summary>
    ///   Lists all available log files.
    /// </summary>
    /// <returns>
    ///   The available log files.
    /// </returns>
    IEnumerable<LogFileInfo> ListFiles();

    /// <summary>
    ///   Reads entries from the current log file using line-based paging.
    /// </summary>
    /// <param name="offset">
    ///   The line offset to start from.
    /// </param>
    /// <param name="limit">
    ///   The maximum number of entries to return.
    /// </param>
    /// <returns>
    ///   The paged read result.
    /// </returns>
    LogReadResult ReadCurrent(int offset = 0, int limit = 100);

    /// <summary>
    ///   Reads entries from the specified log file using line-based paging.
    /// </summary>
    /// <param name="fileId">
    ///   The log file ID.
    /// </param>
    /// <param name="offset">
    ///   The line offset to start from.
    /// </param>
    /// <param name="limit">
    ///   The maximum number of entries to return.
    /// </param>
    /// <returns>
    ///   The paged read result.
    /// </returns>
    LogReadResult ReadFile(string fileId, int offset = 0, int limit = 100);

    /// <summary>
    ///   Reads entries across all readable log files using optional date-range
    ///   filtering and line-based paging.
    /// </summary>
    /// <param name="from">
    ///   Optional start timestamp (inclusive).
    /// </param>
    /// <param name="to">
    ///   Optional end timestamp (inclusive).
    /// </param>
    /// <param name="offset">
    ///   The line offset to start from.
    /// </param>
    /// <param name="limit">
    ///   The maximum number of entries to return.
    /// </param>
    /// <returns>
    ///   The paged read result.
    /// </returns>
    LogReadResult ReadRange(DateTime? from = null, DateTime? to = null, int offset = 0, int limit = 100);

    /// <summary>
    ///   Opens a stream for downloading a specific log file.
    /// </summary>
    /// <param name="fileId">
    ///   The log file ID.
    /// </param>
    /// <param name="decompress">
    ///   If set to <c>true</c> and the file is compressed, then the returned
    ///   stream contains decompressed content.
    /// </param>
    /// <returns>
    ///   The download metadata and stream.
    /// </returns>
    LogDownloadResult OpenDownload(string fileId, bool decompress = false);

    /// <summary>
    ///   Runs log maintenance immediately using current rotation settings.
    /// </summary>
    void RunRotationMaintenance();

    /// <summary>
    ///   Starts scheduled log maintenance.
    /// </summary>
    void StartMaintenance();
}
