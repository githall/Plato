using System.Collections.Generic;
using Plato.Internal.Assets.Abstractions;

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
                    },
                    new Asset()
                    {
                        Url = "/plato.stars/content/css/star.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    }
                }),

                // Staging
                new AssetEnvironment(TargetEnvironment.Staging, new List<Asset>()
                {
                     new Asset()
                    {
                        Url = "/plato.stars/content/js/star.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = "/plato.stars/content/css/star.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    }
                }),

                // Production
                new AssetEnvironment(TargetEnvironment.Production, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.stars/content/js/star.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = "/plato.stars/content/css/star.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    }
                })

            };

        }

    }

}
