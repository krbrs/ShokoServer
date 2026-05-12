using NHibernate;
using Shoko.Server.Data;

namespace Benchmarks.T172;

public sealed record BenchmarkScenarioDefinition(
    string Id,
    string Name,
    string Category,
    string Source,
    bool ReadOnly,
    string Notes,
    Func<ShokoDbContext, int> ExecuteEf,
    Func<ISession, int> ExecuteNh);
