using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Assets.Abstractions;
using Plato.PrettyPrint.Assets;

namespace Plato.PrettyPrint
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