using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Internal.Assets
{

    public class AssetManager : IAssetManager
    {
        private readonly IList<AssetEnvironment> _localAssets = 
            new List<AssetEnvironment>();

        private List<AssetEnvironment> _cachedEnvironment;

        private readonly IEnumerable<IAssetProvider> _assetProviders;
        private readonly ILogger<AssetManager> _logger;

        public AssetManager(
            IEnumerable<IAssetProvider> assetProviders,
            ILogger<AssetManager> logger)
        {
            _assetProviders = assetProviders;
            _logger = logger;
        }

        public IEnumerable<AssetEnvironment> GetAssets()
        {

            if (_cachedEnvironment == null)
            {
                // Check providers for assets           
                _cachedEnvironment = new List<AssetEnvironment>();
                foreach (var provider in _assetProviders)
                {
                    try
                    {
                        _cachedEnvironment.AddRange(provider.GetAssetEnvironments());
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"An exception occurred whilst attempting to execute a resource provider of type {provider.GetType()}.");
                    }
                }
            }

            // Merge assets set via SetAssets();
            if (_localAssets.Count > 0)
            {
                _cachedEnvironment.AddRange(_localAssets);
            }

            return _cachedEnvironment;

        }

        public void SetAssets(IEnumerable<AssetEnvironment> environments)
        {
            foreach (var environment in environments)
            {
                if (!_localAssets.Contains(environment))
                {
                    _localAssets.Add(environment);
                }
            }
        }

    }
}
