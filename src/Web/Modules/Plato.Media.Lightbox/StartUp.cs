using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Assets.Abstractions;
using PlatoCore.Models.Shell;
using Plato.Media.LightBox.Assets;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Media.Lightbox
{

    public class Startup : StartupBase
    {

        private readonly IShellSettings _shellSettings;

        public Startup(IShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            // Client assets
            services.AddScoped<IAssetProvider, AssetProvider>();
        }

    }

}