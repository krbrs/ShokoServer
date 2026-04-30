using System;

#nullable enable
namespace Shoko.Server.API.v3.Models.Relocation.Input;

/// <summary>
/// Query filter for discovering relocation providers.
/// </summary>
public class RelocationDiscoveryFilter
{
    /// <summary>
    /// Optional name search query.
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// Restrict results to a specific plugin.
    /// </summary>
    public Guid? PluginID { get; set; }
}
