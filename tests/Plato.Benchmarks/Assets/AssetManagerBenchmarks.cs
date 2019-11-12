using BenchmarkDotNet.Attributes;
using Plato.Internal.Assets.Abstractions;
using System.Collections.Generic;
using Plato.Internal.Assets;
using Microsoft.Extensions.Logging;

namespace Plato.Benchmarks.Assets
{

    public class AssetManagerBenchmarks
    {

        ILogger<AssetManager> _logger;
        IEnumerable<IAssetProvider> _providers;

        [Params(10, 100, 1_000, 10_000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {

            // Build providers with default assets
            _providers = new List<IAssetProvider>()
            {
                new AssetProviderBase(DefaultAssets.GetDefaultResources())
            };

            // Sample logger
            _logger = new BenchmarkLogger<AssetManager>();

        }

        [Benchmark(Description = "GetAssets")]
        public void GetAssets()
        {

            var assetManager = new AssetManager(_providers, _logger);
            for (var i = 0; i < N; i++)
            {
                assetManager.GetAssets();
            }

        }

    }

}
