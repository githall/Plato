using System.Collections.Generic;
using PlatoCore.Assets.Abstractions;

namespace Plato.Attachments.Assets
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
                        Url = $"/plato.attachments/content/js/attachments.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.attachments/content/js/dropzone.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.attachments/content/css/attachments.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },                    
                    new Asset()
                    {
                        Url = $"/plato.attachments/content/css/file-icons/file-icon-square-o.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },                  
                    new Asset()
                    {
                        Url = $"/plato.attachments/content/css/dropzone.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    }
                }),

                // Staging
                new AssetEnvironment(TargetEnvironment.Staging, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = $"/plato.attachments/content/js/attachments.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.attachments/content/js/dropzone.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.attachments/content/css/attachments.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                    new Asset()
                    {
                        Url = $"/plato.attachments/content/css/file-icons/file-icon-square-o.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                    new Asset()
                    {
                        Url = $"/plato.attachments/content/css/dropzone.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    }
                }),

                // Production
                new AssetEnvironment(TargetEnvironment.Production, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = $"/plato.attachments/content/js/attachments.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.attachments/content/js/dropzone.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.attachments/content/css/attachments.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                    new Asset()
                    {
                        Url = $"/plato.attachments/content/css/file-icons/file-icon-square-o.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                    new Asset()
                    {
                        Url = $"/plato.attachments/content/css/dropzone.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    }
                })

            };

        }

    }

}
