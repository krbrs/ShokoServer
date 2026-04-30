using System.Collections.Generic;
using System.Net;
using Shoko.Server.API.v3.Models.Relocation;

#nullable enable
namespace Shoko.Server.Services.Relocation;

public class RelocationBatchResult
{
    public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;

    public IReadOnlyList<RelocationResult>? Results { get; init; }

    public IReadOnlyDictionary<string, IReadOnlyList<string>>? ValidationErrors { get; init; }

    public string? Message { get; init; }
}
