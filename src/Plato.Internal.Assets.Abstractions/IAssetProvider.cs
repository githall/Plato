using System.Collections.Generic;

namespace Plato.Internal.Assets.Abstractions
{

    public interface IAssetProvider
    {
        IEnumerable<AssetEnvironment> GetAssetEnvironments();
    }

}
