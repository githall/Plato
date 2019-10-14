using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Core.Assets
{

    public class AssetProvider : IAssetProvider
    {

        private readonly IOptions<ThemeOptions> _themeOptions;
        private readonly IOptions<SiteOptions> _siteOptions;

        public AssetProvider(
            IOptions<ThemeOptions> themeOptions,
            IOptions<SiteOptions> siteOptions)
        {
            _themeOptions = themeOptions;
            _siteOptions = siteOptions;
        }

        public Task<IEnumerable<AssetEnvironment>> GetAssetEnvironments()
        {

            var path = !string.IsNullOrEmpty(_siteOptions.Value.Theme)
                ? _siteOptions.Value.Theme
                : $"{_themeOptions.Value.VirtualPathToThemesFolder}/default";

            IEnumerable<AssetEnvironment> assets = new List<AssetEnvironment>
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

            return Task.FromResult(assets);

        }

    }

}
