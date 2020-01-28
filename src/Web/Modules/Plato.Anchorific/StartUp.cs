using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Assets.Abstractions;
using Plato.Anchorific.Assets;

namespace Plato.Anchorific
{

    public class Startup : StartupBase
    {

        public override void ConfigureServices(IServiceCollection services)
        {
            // Client assets
            services.AddScoped<IAssetProvider, AssetProvider>();
        }

    }

}