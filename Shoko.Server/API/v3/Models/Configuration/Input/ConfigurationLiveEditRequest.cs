using Newtonsoft.Json.Linq;
using Shoko.Abstractions.Config.Enums;

#nullable enable
namespace Shoko.Server.API.v3.Models.Configuration.Input;

/// <summary>
/// Request body for invoking a live-edit configuration action.
/// </summary>
public class ConfigurationLiveEditRequest
{
    /// <summary>
    /// The configuration document to apply the live edit to.
    /// </summary>
    public JToken Configuration { get; set; } = new JObject();

    /// <summary>
    /// Optional configuration path to target.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// The type of live-edit event to execute.
    /// </summary>
    public ReactiveEventType ReactiveEventType { get; set; } = ReactiveEventType.All;
}
