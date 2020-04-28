using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using Plato.Dropzone.Assets;
using PlatoCore.Assets.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Dropzone
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
            // Register client assets
            services.AddScoped<IAssetProvider, AssetProvider>();

        }

    }

}