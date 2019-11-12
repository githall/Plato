using System;
using BenchmarkDotNet.Attributes;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Benchmarks.Abstractions.Extensions
{

    public class DateTimeBenchmarks
    {

        [Params(10, 100, 1_000, 10_000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
        }

        [Benchmark(Description = "ToPrettyDate")]
        public void ToPrettyDate()
        {
            for (var i = 0; i < N; i++)
            {
                DateTime.Now.ToPrettyDate();
            }
        }

        [Benchmark(Description = "DayDifference")]
        public void DayDifference()
        {
            for (var i = 0; i < N; i++)
            {
                DateTime.Now.DayDifference(DateTime.Now.AddDays(365));
            }
        }

        [Benchmark(Description = "ToSortableDateTimePattern")]
        public void ToSortableDateTimePattern()
        {
            for (var i = 0; i < N; i++)
            {
                DateTime.Now.ToSortableDateTimePattern();
            }
        }

        [Benchmark(Description = "Floor")]
        public void Floor()
        {
            for (var i = 0; i < N; i++)
            {
                DateTime.Now.Floor();
            }
        }

        [Benchmark(Description = "Ceil")]
        public void Ceil()
        {
            for (var i = 0; i < N; i++)
            {
                DateTime.Now.Ceil();
            }
        }

    }

}
