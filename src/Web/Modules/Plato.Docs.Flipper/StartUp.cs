using Microsoft.Extensions.DependencyInjection;
using Plato.Docs.Flipper.Navigation;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Docs.Flipper
{

    public class Startup : StartupBase
    {
        
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<INavigationProvider, DocFooterMenu>();
        }

    }

}