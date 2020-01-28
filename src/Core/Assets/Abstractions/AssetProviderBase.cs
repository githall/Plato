using System.Collections.Generic;

namespace PlatoCore.Assets.Abstractions
{

    public class AssetProviderBase : IAssetProvider
    {

        private readonly IEnumerable<AssetEnvironment> _environments;

        public AssetProviderBase(IEnumerable<AssetEnvironment> environments)
        {
            _environments = environments;
        }

        public virtual IEnumerable<AssetEnvironment> GetAssetEnvironments() => _environments;

    }

}
