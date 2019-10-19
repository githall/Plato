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

        private List<AssetEnvironment> _environments;

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

            if (_environments == null)
            {
                // Check providers for assets           
                _environments = new List<AssetEnvironment>();
                foreach (var provider in _assetProviders)
                {
                    try
                    {
                        _environments.AddRange(provider.GetAssetEnvironments());
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
                _environments.AddRange(_localAssets);
            }

            return _environments;

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
