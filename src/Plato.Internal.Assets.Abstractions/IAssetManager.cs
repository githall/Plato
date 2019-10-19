using System.Collections.Generic;

namespace Plato.Internal.Assets.Abstractions
{
    public interface IAssetManager
    {

        IEnumerable<AssetEnvironment> GetAssets();

        void SetAssets(IEnumerable<AssetEnvironment> environments);

    }


}
