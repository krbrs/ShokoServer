using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;

#nullable enable
namespace Shoko.Server.API.v3.Models.Relocation.Input;

/// <summary>
///   Represents the information required to preview a batch file relocation
///   before committing to it.
/// </summary>
public class BatchRelocatePreviewBody
{
    /// <summary>
    ///   The file IDs to preview.
    /// </summary>
    [Required]
    public IEnumerable<int> FileIDs { get; set; } = [];

    /// <summary>
    ///   The provider ID. Omit to use the default provider with the default
    ///   configuration.
    /// </summary>
    public Guid? ProviderID { get; set; }

    /// <summary>
    ///   The configuration to use if the provider requires one.
    /// </summary>
    public JObject? Configuration { get; set; }

    /// <summary>
    ///   Whether or not to move the files. If omitted, defaults to the relocation service setting.
    /// </summary>
    public bool? Move { get; set; }

    /// <summary>
    ///   Whether or not to rename the files. If omitted, defaults to the relocation service setting.
    /// </summary>
    public bool? Rename { get; set; }

    /// <summary>
    ///   Whether or not to allow relocation inside the destination. If omitted, defaults to the relocation service setting.
    /// </summary>
    public bool? AllowRelocationInsideDestination { get; set; }
}
