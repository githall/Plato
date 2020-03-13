using System.Collections.Generic;
using PlatoCore.Assets.Abstractions;

namespace Plato.Files.Assets
{
    public class AssetProvider : IAssetProvider
    {

        // Plato.Files uses the awesome file icons font library...
        // https://github.com/dmhendricks/file-icon-vectors

        public IEnumerable<AssetEnvironment> GetAssetEnvironments()
        {

            return new List<AssetEnvironment>
            {

                // Development
                new AssetEnvironment(TargetEnvironment.Development, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = $"/plato.files/content/js/files.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },                
                    new Asset()
                    {
                        Url = $"/plato.files/content/css/files.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },                    
                    new Asset()
                    {
                        Url = $"/plato.Files/content/css/file-icons/file-icon-square-o.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    }
                }),

                // Staging
                new AssetEnvironment(TargetEnvironment.Staging, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = $"/plato.files/content/js/files.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },               
                    new Asset()
                    {
                        Url = $"/plato.files/content/css/files.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                    new Asset()
                    {
                        Url = $"/plato.files/content/css/file-icons/file-icon-square-o.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    }
                }),

                // Production
                new AssetEnvironment(TargetEnvironment.Production, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = $"/plato.files/content/js/files.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.Files/content/css/files.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                    new Asset()
                    {
                        Url = $"/plato.files/content/css/file-icons/file-icon-square-o.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    }
                })

            };

        }

    }

}
