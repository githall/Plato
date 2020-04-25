using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Assets.Abstractions;
using Plato.PrettyPrint.Assets;
using PlatoCore.Hosting.Abstractions;

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