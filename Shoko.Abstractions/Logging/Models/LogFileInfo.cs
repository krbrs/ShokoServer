using System;

namespace Shoko.Abstractions.Logging.Models;

/// <summary>
///   Metadata about a log file available for reading or downloading.
/// </summary>
public class LogFileInfo
{
    /// <summary>
    ///   Stable file identifier used in API/service methods.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    ///   Display file name.
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    ///   Full file-system path.
    /// </summary>
    public required string FullPath { get; init; }

    /// <summary>
    ///   File size in bytes.
    /// </summary>
    public required long Size { get; init; }

    /// <summary>
    ///   Last modification timestamp in UTC.
    /// </summary>
    public required DateTime LastModifiedAt { get; init; }

    /// <summary>
    ///   Indicates whether the file is compressed.
    /// </summary>
    public required bool IsCompressed { get; init; }

    /// <summary>
    ///   Detected content format.
    /// </summary>
    public required LogFileFormat Format { get; init; }
}
