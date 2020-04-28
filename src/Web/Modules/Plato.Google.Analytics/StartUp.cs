using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Layout.ActionFilters;
using Plato.Google.Analytics.ActionFilters;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Google.Analytics
{

    public class Startup : StartupBase
    {

        public override void ConfigureServices(IServiceCollection services)
        {
            // Action filter
            services.AddScoped<IModularActionFilter, GoogleAnalyticsFilter>();
        }

    }

}