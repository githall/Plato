using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Assets.Abstractions;
using Plato.Anchorific.Assets;
using PlatoCore.Hosting.Abstractions;

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