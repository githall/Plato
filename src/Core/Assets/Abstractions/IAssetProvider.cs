using System.Collections.Generic;

namespace PlatoCore.Assets.Abstractions
{

    public interface IAssetProvider
    {
        IEnumerable<AssetEnvironment> GetAssetEnvironments();
    }

}
