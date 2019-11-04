using System.Collections.Generic;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Mentions.Assets
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
                        Url = "/plato.mentions/content/css/mentions.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = constraints
                    },
                    new Asset()
                    {
                        Url = "/plato.mentions/content/js/mentions.js",
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
                        Url = "/plato.mentions/content/css/mentions.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = constraints
                    },
                    new Asset()
                    {
                        Url = "/plato.mentions/content/js/mentions.min.js",
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
                        Url = "/plato.mentions/content/css/mentions.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = constraints
                    },
                    new Asset()
                    {
                        Url = "/plato.mentions/content/js/mentions.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = constraints
                    }

                })

            };

        }

    }

}
