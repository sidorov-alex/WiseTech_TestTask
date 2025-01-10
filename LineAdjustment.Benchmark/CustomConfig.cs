using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;

namespace LineAdjustment.Benchmark;

class CustomConfig : ManualConfig
{
    public CustomConfig()
    {
        Add(MemoryDiagnoser.Default);
    }
}
