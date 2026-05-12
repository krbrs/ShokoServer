using BenchmarkDotNet.Attributes;

namespace Benchmarks.T172;

[MemoryDiagnoser]
[BenchmarkCategory("T172", "EFCore")]
public class T172EfCoreBenchmarks
{
    private BenchmarkDatabaseHarness _harness = null!;

    [ParamsSource(nameof(ScenarioIds))]
    public string ScenarioId { get; set; } = string.Empty;

    public IEnumerable<string> ScenarioIds => BenchmarkHarnessSettings.LoadFromEnvironment().ResolveScenarioIds();

    [GlobalSetup]
    public void Setup()
    {
        var settings = BenchmarkHarnessSettings.LoadFromEnvironment();
        settings.ValidateForDatabaseBenchmarks();
        _harness = new BenchmarkDatabaseHarness(settings);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _harness.Dispose();
    }

    [Benchmark]
    public int ExecuteScenario()
        => _harness.ExecuteEfScenario(ScenarioId);
}
