using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shoko.Abstractions.Logging.Models;

namespace Shoko.Abstractions.Logging.Services;

/// <summary>
///   Service responsible for listing, reading, downloading, and maintaining
///   server log files.
/// </summary>
public interface ILogService
{
    #region Maintenance

    /// <summary>
    ///   Starts scheduled log maintenance.
    /// </summary>
    void StartMaintenance();

    /// <summary>
    ///   Runs log maintenance immediately using current rotation settings.
    /// </summary>
    void RunRotationMaintenance();

    #endregion

    #region Log File Operations

    /// <summary>
    ///   Lists all available log files, in most recent order, with the current
    ///   log file first.
    /// </summary>
    /// <returns>
    ///   The available log files.
    /// </returns>
    IReadOnlyList<LogFileInfo> GetAllLogFiles();

    /// <summary>
    ///   Gets the current log file info.
    /// </summary>
    /// <returns>
    ///   The current log file info.
    /// </returns>
    LogFileInfo GetCurrentLogFile();

    /// <summary>
    ///   Gets the log file info by it's identifier.
    /// </summary>
    /// <param name="fileID">
    ///   The log file identifier.
    /// </param>
    /// <returns>
    ///   The log file info, or <c>null</c> if not found.
    /// </returns>
    LogFileInfo? GetLogFileByID(Guid fileID);

    /// <summary>
    ///   Reads entries from the specified log file using line-based paging in
    ///   ascending or descending order.
    /// </summary>
    /// <param name="fileInfo">
    ///   The log file to read.
    /// </param>
    /// <param name="offset">
    ///   Optional. The line offset to start from.
    /// </param>
    /// <param name="limit">
    ///   Optional. The maximum number of entries to return. Set to <c>0</c> to
    ///   disable limit.
    /// </param>
    /// <param name="descending">
    ///   Optional. If set to <c>true</c>, then the entries are returned in
    ///   descending order.
    /// </param>
    /// <returns>
    ///   The paged read result, with the first entry being the least recent in
    ///   ascending order, or the most recent in descending order.
    /// </returns>
    LogReadResult ReadLogFile(
        LogFileInfo fileInfo,
        [Range(0, uint.MaxValue)] uint offset = 0,
        [Range(0, 1000)] uint limit = 100,
        bool descending = false
    );

    /// <summary>
    ///   Opens a stream for downloading a specific log file.
    /// </summary>
    /// <param name="fileInfo">
    ///   The log file to download.
    /// </param>
    /// <param name="format">
    ///   Format to return the log entries as. Only applies to
    ///   <seealso cref="LogFileFormat.JsonL">JSONL-formatted</seealso> files.
    ///   Can be <c>"simple"</c>, <c>"full"</c>, <c>"json"</c>, or
    ///   <c>"legacy"</c>.
    /// </param>
    /// <returns>
    ///   The download metadata and stream.
    /// </returns>
    LogDownloadResult DownloadLogFile(LogFileInfo fileInfo, string format = "simple");

    /// <summary>
    ///   Deletes the specified log file.
    /// </summary>
    /// <param name="fileInfo">
    ///   The log file to delete.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///   Thrown when attempting to delete the current log file.
    /// </exception>
    /// <returns>
    ///   <c>true</c> if the file was deleted; otherwise, <c>false</c>.
    /// </returns>
    void DeleteLogFile(LogFileInfo fileInfo);

    #endregion

    #region Range Operations

    /// <summary>
    ///   Reads entries across all readable log files in descending order.
    ///   With optional date-range filtering and line-based paging.
    /// </summary>
    /// <param name="from">
    ///   Optional. Earliest timestamp (inclusive). Defaults to
    ///   <seealso cref="DateTime.MinValue"/> if not set.
    /// </param>
    /// <param name="to">
    ///   Optional. Latest timestamp (inclusive). Defaults to
    ///   <seealso cref="DateTime.UtcNow"/> if not set.
    /// </param>
    /// <param name="offset">
    ///   Optional. The line offset to start from.
    /// </param>
    /// <param name="limit">
    ///   Optional. The maximum number of entries to return. Set to <c>0</c> to
    ///   disable limit.
    /// </param>
    /// <param name="descending">
    ///   Optional. If set to <c>true</c>, then the entries are returned in
    ///   descending order. Defaults to <c>true</c>.
    /// </param>
    /// <returns>
    ///   The paged read result, with the first entry being the most recent.
    /// </returns>
    LogReadResult ReadRange(
        DateTime? from = null,
        DateTime? to = null,
        [Range(0, uint.MaxValue)] uint offset = 0,
        [Range(0, 1000)] uint limit = 100,
        bool descending = true
    );

    /// <summary>
    ///   Opens a stream for downloading a specific log file.
    /// </summary>
    /// <param name="from">
    ///   Optional. Earliest timestamp (inclusive). Defaults to
    ///   <seealso cref="DateTime.MinValue"/> if not set.
    /// </param>
    /// <param name="to">
    ///   Optional. The latest timestamp (inclusive). Defaults to
    ///   <seealso cref="DateTime.UtcNow"/> if not set.
    /// </param>
    /// <param name="format">
    ///   Format to return the log entries as. Only applies to
    ///   <seealso cref="LogFileFormat.JsonL">JSONL-formatted</seealso> files.
    ///   Can be <c>"simple"</c>, <c>"full"</c>, <c>"json"</c>, or
    ///   <c>"legacy"</c>.
    /// </param>
    /// <returns>
    ///   The download metadata and stream.
    /// </returns>
    LogDownloadResult DownloadRange(DateTime? from = null, DateTime? to = null, string format = "simple");

    #endregion
}
