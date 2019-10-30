using System.Collections.Generic;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Docs.Anchorific.Assets
{
    public class AssetProvider : IAssetProvider
    {

        public IEnumerable<AssetEnvironment> GetAssetEnvironments()
        {

            return new List<AssetEnvironment>
            {

                // Development
                new AssetEnvironment(TargetEnvironment.Development, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.docs.anchorific/content/js/anchorific.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    }
                }),

                // Staging
                new AssetEnvironment(TargetEnvironment.Staging, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.docs.anchorific/content/js/anchorific.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    }
                }),

                // Production
                new AssetEnvironment(TargetEnvironment.Production, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.docs.anchorific/content/js/anchorific.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    }
                })

            };

        }

    }

}
