using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shoko.Abstractions.Extensions;

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
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public required LogLevel Level { get; init; }

    /// <summary>
    ///   Thread ID associated with the entry.
    /// </summary>
    public required int ThreadId { get; init; }

    /// <summary>
    ///   Process ID associated with the entry.
    /// </summary>
    public required int ProcessId { get; init; }

    /// <summary>
    ///   Logger category or name.
    /// </summary>
    public required string Logger { get; init; }

    /// <summary>
    ///   Caller information from the logging pipeline.
    /// </summary>
    public required string Caller { get; init; }

    /// <summary>
    ///   Rendered log message.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    ///   Optional rendered exception information.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Exception { get; init; }

    /// <inheritdoc/>
    public override string ToString()
        => ToString("simple");

    /// <summary>
    ///   Returns a string representation of the log entry.
    /// </summary>
    /// <param name="format">
    ///   The format to use. Can be <c>"simple"</c>, <c>"full"</c>,
    ///   <c>"json"</c>, or <c>"legacy"</c>.
    /// </param>
    /// <returns>
    ///   A string representation of the log entry.
    /// </returns>
    public string ToString(string? format)
        => format switch
        {
            "simple" => $"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level.ToShortString()}] {Logger.Split('.').Last()}: {Message}{(Exception is { Length: > 0 } ? $": {Exception}" : string.Empty)}",
            "full" => $"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level.ToShortString()}] [{ThreadId:000}] {Logger}: {Message}{(Exception is { Length: > 0 } ? Environment.NewLine + Exception : string.Empty)}",
            "json" => System.Text.Json.JsonSerializer.Serialize(this),
            "legacy" => $"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] {Level.ToNLogString()}|{Logger} > {Message}{(Exception is { Length: > 0 } ? $": {Exception}" : string.Empty)}",
            _ => throw new ArgumentException($"Invalid format: {format}"),
        };
}
