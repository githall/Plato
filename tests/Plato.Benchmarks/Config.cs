using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;

namespace Plato.Benchmarks
{
    public class Config : ManualConfig
    {
        public Config()
        {
            this.Add(MemoryDiagnoser.Default);
        }

        public class ShortClr : Config
        {
            public ShortClr()
            {
                this.Add(
                    Job.Default.WithLaunchCount(1).WithWarmupCount(3).WithIterationCount(3),
                    Job.Default.WithLaunchCount(1).WithWarmupCount(3).WithIterationCount(3)
                );
            }
        }
    }

}
