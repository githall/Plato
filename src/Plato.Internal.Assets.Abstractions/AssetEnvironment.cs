using System.Linq;
using System.Collections.Generic;

namespace Plato.Internal.Assets.Abstractions
{

    public class AssetEnvironment
    {

        public TargetEnvironment TargetEnvironment { get; set; }

        public IList<Asset> Resources { get; set; }

        public AssetEnvironment(TargetEnvironment env, IEnumerable<Asset> resources)
        {
            this.TargetEnvironment = env;
            this.Resources = resources.ToList();
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
