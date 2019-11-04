using System.Collections.Generic;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Markdown.Assets
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
                        Url = $"/plato.markdown/content/js/markdown.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/js/dropzone.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/css/markdown.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/css/dropzone.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    }
                }),

                // Staging
                new AssetEnvironment(TargetEnvironment.Staging, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/js/markdown.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/js/dropzone.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/css/markdown.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/css/dropzone.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    }
                }),

                // Production
                new AssetEnvironment(TargetEnvironment.Production, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/js/markdown.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/js/dropzone.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/css/markdown.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/css/dropzone.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    }
                })

            };

        }

    }

}
