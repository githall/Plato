using BenchmarkDotNet.Attributes;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Benchmarks.Abstractions.Extensions
{
    public class Array
    {

        [Benchmark(Description = "Plato.Internal.Abstractions.Extensions.Array.ToDelimitedString()")]
        public void ToDelimitedString()
        {
            var sampleData = new int[] {
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

            for (var i = 0; i < 50000; i++)
            {
                sampleData.ToDelimitedString();
            }

        }

    }

}
