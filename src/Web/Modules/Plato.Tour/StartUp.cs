using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Tour.ViewProviders;
using Plato.Core.Models;
using PlatoCore.Layout.ViewProviders;
using Plato.Tour.Configuration;
using PlatoCore.Models.Tour;
using Microsoft.Extensions.Options;
using PlatoCore.Messaging.Abstractions;
using Plato.Tour.Subscribers;

namespace Plato.Tour
{

    public class Startup : StartupBase
    {

        public override void ConfigureServices(IServiceCollection services)
        {

            // Homepage view providers
            services.AddScoped<IViewProviderManager<HomeIndex>, ViewProviderManager<HomeIndex>>();
            services.AddScoped<IViewProvider<HomeIndex>, HomeViewProvider>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, FeatureSubscriber>();

            // Configuration
            // services.AddTransient<IConfigureOptions<TourDescriptor>, TourDescriptorConfiguration>();

        }

    }

}