using System;
using Shoko.Abstractions.Logging.Models;

#nullable enable
namespace Shoko.Server.API.v3.Models.Logging;

/// <summary>
/// API model containing log file metadata.
/// </summary>
public class LogFile
{
    /// <summary>
    /// Log file identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// File name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long Size { get; }

    /// <summary>
    /// Indicates whether the file is compressed.
    /// </summary>
    public bool IsCompressed { get; }

    /// <summary>
    /// Detected file format.
    /// </summary>
    public string Format { get; }

    /// <summary>
    /// Last modified timestamp in UTC.
    /// </summary>
    public DateTime LastModifiedAt { get; }

    public LogFile(LogFileInfo file)
    {
        Id = file.Id;
        Name = file.FileName;
        Size = file.Size;
        IsCompressed = file.IsCompressed;
        Format = file.Format.ToString().ToLowerInvariant();
        LastModifiedAt = file.LastModifiedAt;
    }
}
