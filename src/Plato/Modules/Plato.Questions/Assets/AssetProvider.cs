using System.Collections.Generic;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Questions.Assets
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
                        ["area"] = "Plato.Questions"
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
                        Url = "/plato.questions/content/css/questions.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = constraints
                    },
                    new Asset()
                    {
                        Url = "/plato.questions/content/js/questions.js",
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
                        Url = "/plato.questions/content/css/questions.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = constraints
                    },
                    new Asset()
                    {
                        Url = "/plato.questions/content/js/questions.js",
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
                        Url = "/plato.questions/content/css/questions.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = constraints
                    },
                    new Asset()
                    {
                        Url = "/plato.questions/content/js/questions.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = constraints
                    }
                })

            };

        }

    }

}
