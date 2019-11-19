using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ActionFilters;
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