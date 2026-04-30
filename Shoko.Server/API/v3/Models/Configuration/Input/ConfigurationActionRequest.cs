using Newtonsoft.Json.Linq;

#nullable enable
namespace Shoko.Server.API.v3.Models.Configuration.Input;

/// <summary>
/// Request body for invoking a custom configuration action.
/// </summary>
public class ConfigurationActionRequest
{
    /// <summary>
    /// Optional configuration document to use when invoking the action.
    /// </summary>
    public JToken? Configuration { get; set; }

    /// <summary>
    /// Optional configuration path to target.
    /// </summary>
    public string Path { get; set; } = string.Empty;
}
