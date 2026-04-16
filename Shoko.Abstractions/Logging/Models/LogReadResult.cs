using System.Collections.Generic;

namespace Shoko.Abstractions.Logging.Models;

/// <summary>
///   Paged log read result.
/// </summary>
public class LogReadResult
{
    /// <summary>
    ///   Next line offset to continue reading from.
    /// </summary>
    public required int NextOffset { get; init; }

    /// <summary>
    ///   Returned log entries for the requested page.
    /// </summary>
    public required IReadOnlyList<LogEntry> Entries { get; init; }
}
