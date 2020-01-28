using System.Collections.Generic;
using PlatoCore.Assets.Abstractions;

namespace Plato.Entities.History.Assets
{
    public class AssetProvider : IAssetProvider
    {

        public IEnumerable<AssetEnvironment> GetAssetEnvironments()
        {


            var constraints = new AssetConstraints()
            {
                Routes = new List<AssetConstraint>() {
                    new AssetConstraint()
                    {
                        ["area"] = "Plato.*",
                        ["controller"] = "Home",
                        ["action"] = "Display",
                    }
                }
            };

            return new List<AssetEnvironment>
            {

                // Development
                new AssetEnvironment(TargetEnvironment.Development, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.entities.history/content/css/history.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = constraints
                    },
                    new Asset()
                    {
                        Url = "/plato.entities.history/content/js/history.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = constraints
                    }
                }),

                // Staging
                new AssetEnvironment(TargetEnvironment.Staging, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.entities.history/content/css/history.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = constraints
                    },
                    new Asset()
                    {
                        Url = "/plato.entities.history/content/js/history.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = constraints
                    }
                }),

                // Production
                new AssetEnvironment(TargetEnvironment.Production, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.entities.history/content/css/history.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = constraints
                    },
                    new Asset()
                    {
                        Url = "/plato.entities.history/content/js/history.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = constraints
                    }

                })

            };

        }

    }

}
