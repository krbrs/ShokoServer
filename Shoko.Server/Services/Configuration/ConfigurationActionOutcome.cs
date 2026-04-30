using System.Collections.Generic;
using System.Net;
using Shoko.Server.API.v3.Models.Configuration;

#nullable enable
namespace Shoko.Server.Services.Configuration;

public class ConfigurationActionOutcome
{
    public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;

    public ConfigurationActionResult? Result { get; init; }

    public IReadOnlyDictionary<string, IReadOnlyList<string>>? ValidationErrors { get; init; }

    public string? Message { get; init; }
}
