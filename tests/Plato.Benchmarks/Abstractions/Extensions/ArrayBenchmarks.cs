using BenchmarkDotNet.Attributes;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Benchmarks.Abstractions.Extensions
{

    // https://github.com/dotnet/BenchmarkDotNet

    public class ArrayBenchmarks
    {

        private int[] _intData = null;
        private string[] _stringData = null;

        [Params(10, 100, 1_000, 10_000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            _intData = new int[] {
                1,
                2,
                3,
                4,
                5,
                6,
                7,
                8,
                9,
                0
            };

            _stringData = new string[]
            {
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "0"
            };

        }

        [Benchmark(Description = "IntArrayToDelimitedString")]
        public void IntArrayToDelimitedString()
        {           
            for (var i = 0; i < N; i++)
            {
                _intData.ToDelimitedString();
            }
        }

        [Benchmark(Description = "StringArrayToDelimitedString")]
        public void StringArrayToDelimitedString()
        {
            for (var i = 0; i < N; i++)
            {
                _stringData.ToDelimitedString();
            }
        }

    }

}
