using System.Linq;
using System.Collections.Generic;

namespace PlatoCore.Assets.Abstractions
{

    public class AssetEnvironment
    {

        public TargetEnvironment TargetEnvironment { get; set; }

        public IList<Asset> Assets { get; set; }

        public AssetEnvironment(TargetEnvironment env, IEnumerable<Asset> assets)
        {
            TargetEnvironment = env;
            Assets = assets.ToList();
        }

    }

    public enum TargetEnvironment
    {
        All,
        Development,
        Staging,
        Production
    }

}
