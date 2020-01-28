using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ActionFilters;
using Plato.Google.Analytics.ActionFilters;

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