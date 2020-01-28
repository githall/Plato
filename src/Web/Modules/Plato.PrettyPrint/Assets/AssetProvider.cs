using System.Collections.Generic;
using PlatoCore.Assets.Abstractions;

namespace Plato.PrettyPrint.Assets
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
                    },
                    new AssetConstraint()
                    {
                        ["area"] = "Plato.*",
                        ["controller"] = "Home",
                        ["action"] = "Create",
                    },
                    new AssetConstraint()
                    {
                        ["area"] = "Plato.*",
                        ["controller"] = "Home",
                        ["action"] = "Edit",
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
                        Url = "/plato.prettyprint/content/js/prettify.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = constraints
                    },
                    new Asset()
                    {
                        Url = "/plato.prettyprint/content/js/prettyprint.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = constraints
                    },
                    new Asset()
                    {
                        Url = "/plato.prettyprint/content/css/prettyprint.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = constraints
                    }
                }),

                // Staging
                new AssetEnvironment(TargetEnvironment.Staging, new List<Asset>()
                {
                     new Asset()
                    {
                        Url = "/plato.prettyprint/content/js/prettify.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = constraints
                    },
                    new Asset()
                    {
                        Url = "/plato.prettyprint/content/js/prettyprint.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = constraints
                    },
                    new Asset()
                    {
                        Url = "/plato.prettyprint/content/css/prettyprint.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = constraints
                    }
                }),

                // Production
                new AssetEnvironment(TargetEnvironment.Production, new List<Asset>()
                {
                     new Asset()
                    {
                        Url = "/plato.prettyprint/content/js/prettify.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = constraints
                    },
                    new Asset()
                    {
                        Url = "/plato.prettyprint/content/js/prettyprint.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = constraints
                    },
                    new Asset()
                    {
                        Url = "/plato.prettyprint/content/css/prettyprint.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = constraints
                    }
                })

            };

        }

    }

}
