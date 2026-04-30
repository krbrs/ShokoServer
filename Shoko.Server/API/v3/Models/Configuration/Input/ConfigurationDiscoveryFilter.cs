using System;
using Shoko.Server.API.v3.Models.Common;

#nullable enable
namespace Shoko.Server.API.v3.Models.Configuration.Input;

/// <summary>
/// Query filter for discovering registered configuration entries.
/// </summary>
public class ConfigurationDiscoveryFilter
{
    /// <summary>
    /// Optional name search query.
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// Restrict results to a specific plugin.
    /// </summary>
    public Guid? PluginID { get; set; }

    /// <summary>
    /// Whether hidden configurations should be included, excluded, or only returned.
    /// </summary>
    public IncludeOnlyFilter Hidden { get; set; } = IncludeOnlyFilter.True;

    /// <summary>
    /// Whether base configurations should be included, excluded, or only returned.
    /// </summary>
    public IncludeOnlyFilter IsBase { get; set; } = IncludeOnlyFilter.True;

    /// <summary>
    /// Whether configurations with custom new factories should be included, excluded, or only returned.
    /// </summary>
    public IncludeOnlyFilter CustomNewFactory { get; set; } = IncludeOnlyFilter.True;

    /// <summary>
    /// Whether configurations with custom validation should be included, excluded, or only returned.
    /// </summary>
    public IncludeOnlyFilter CustomValidation { get; set; } = IncludeOnlyFilter.True;

    /// <summary>
    /// Whether configurations with custom actions should be included, excluded, or only returned.
    /// </summary>
    public IncludeOnlyFilter CustomActions { get; set; } = IncludeOnlyFilter.True;

    /// <summary>
    /// Whether configurations with custom save behavior should be included, excluded, or only returned.
    /// </summary>
    public IncludeOnlyFilter CustomSave { get; set; } = IncludeOnlyFilter.True;

    /// <summary>
    /// Whether configurations with custom load behavior should be included, excluded, or only returned.
    /// </summary>
    public IncludeOnlyFilter CustomLoad { get; set; } = IncludeOnlyFilter.True;

    /// <summary>
    /// Whether configurations with live-edit support should be included, excluded, or only returned.
    /// </summary>
    public IncludeOnlyFilter LiveEdit { get; set; } = IncludeOnlyFilter.True;
}
