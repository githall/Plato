using System.Collections.Generic;
using PlatoCore.Assets.Abstractions;

namespace Plato.Stars.Assets
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
                        Url = "/plato.stars/content/js/star.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    }
                }),

                // Staging
                new AssetEnvironment(TargetEnvironment.Staging, new List<Asset>()
                {
                     new Asset()
                    {
                        Url = "/plato.stars/content/js/star.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    }
                }),

                // Production
                new AssetEnvironment(TargetEnvironment.Production, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.stars/content/js/star.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    }
                })

            };

        }

    }

}
