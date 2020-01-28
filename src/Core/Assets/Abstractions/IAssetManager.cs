using System.Collections.Generic;

namespace PlatoCore.Assets.Abstractions
{
    public interface IAssetManager
    {

        IEnumerable<AssetEnvironment> GetAssets();

        void SetAssets(IEnumerable<AssetEnvironment> environments);

    }


}
