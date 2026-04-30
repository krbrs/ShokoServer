using System.Collections.Generic;
using System.Net;

#nullable enable
namespace Shoko.Server.Services.Relocation;

public class RelocationPipeDocumentResult
{
    public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;

    public string? Content { get; init; }

    public string ContentType { get; init; } = "application/json";

    public IReadOnlyDictionary<string, IReadOnlyList<string>>? ValidationErrors { get; init; }

    public string? Message { get; init; }
}
