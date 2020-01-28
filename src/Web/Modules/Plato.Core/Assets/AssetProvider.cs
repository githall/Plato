using System.Collections.Generic;
using PlatoCore.Assets.Abstractions;
using PlatoCore.Theming.Abstractions;

namespace Plato.Core.Assets
{

    public class AssetProvider : IAssetProvider
    {

        private readonly IThemeSelector _themeSelector;
        
        public AssetProvider(IThemeSelector themeSelector)
        {
            _themeSelector = themeSelector;
        }

        public IEnumerable<AssetEnvironment> GetAssetEnvironments()
        {

            var path = _themeSelector.GetThemePath();
            return new List<AssetEnvironment>
            {

                // Development
                new AssetEnvironment(TargetEnvironment.Development, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = $"/{path}/theme.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Order = int.MaxValue
                    }
                }),

                // Staging
                new AssetEnvironment(TargetEnvironment.Staging, new List<Asset>()
                {
                    /* Css */
                    new Asset()
                    {
                        Url = $"/{path}/theme.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Order = int.MaxValue
                    },
                }),

                // Production
                new AssetEnvironment(TargetEnvironment.Production, new List<Asset>()
                {
                    /* Css */
                    new Asset()
                    {
                        Url = $"/{path}/theme.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Order = int.MaxValue
                    },
                })

            };

        }

    }

}
