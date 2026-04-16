using System;
using System.Text.Json;

namespace Shoko.Abstractions.Logging.Models;

/// <summary>
///   A structured log entry parsed from JSONL log files.
/// </summary>
public class LogEntry
{
    /// <summary>
    ///   Entry timestamp in UTC.
    /// </summary>
    public required DateTime Timestamp { get; init; }

    /// <summary>
    ///   Log level.
    /// </summary>
    public required string Level { get; init; }

    /// <summary>
    ///   Logger category or name.
    /// </summary>
    public required string Logger { get; init; }

    /// <summary>
    ///   Caller information from the logging pipeline.
    /// </summary>
    public required string Caller { get; init; }

    /// <summary>
    ///   Thread ID associated with the entry.
    /// </summary>
    public required int ThreadId { get; init; }

    /// <summary>
    ///   Process ID associated with the entry.
    /// </summary>
    public required int ProcessId { get; init; }

    /// <summary>
    ///   Rendered log message.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    ///   Optional rendered exception information.
    /// </summary>
    public string? Exception { get; init; }
}
