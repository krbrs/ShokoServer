using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable enable
namespace Shoko.Server.API.v3.Models.Relocation.Input;

/// <summary>
/// Represents the information required to relocate a batch of files.
/// </summary>
public class BatchRelocateBody
{
    /// <summary>
    /// The file IDs to relocate.
    /// </summary>
    [Required]
    public IEnumerable<int> FileIDs { get; set; } = [];

    /// <summary>
    /// Indicates whether empty directories should be deleted after relocation.
    /// </summary>
    public bool DeleteEmptyDirectories { get; set; } = true;

    /// <summary>
    /// Whether or not to move the files. If omitted, defaults to the relocation service setting.
    /// </summary>
    public bool? Move { get; set; }

    /// <summary>
    /// Whether or not to rename the files. If omitted, defaults to the relocation service setting.
    /// </summary>
    public bool? Rename { get; set; }

    /// <summary>
    /// Whether or not to allow relocation inside the destination. If omitted, defaults to the relocation service setting.
    /// </summary>
    public bool? AllowRelocationInsideDestination { get; set; }
}
