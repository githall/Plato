using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Tour.ViewProviders;
using Plato.Core.Models;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using Plato.Tour.Subscribers;
using Plato.Admin.Models;
using PlatoCore.Models.Tour;
using Plato.Tour.Configuration;

namespace Plato.Tour
{

    public class Startup : StartupBase
    {

        public override void ConfigureServices(IServiceCollection services)
        {

            // Homepage view providers
            services.AddScoped<IViewProviderManager<HomeIndex>, ViewProviderManager<HomeIndex>>();
            services.AddScoped<IViewProvider<HomeIndex>, HomeViewProvider>();

            // Admin index View providers
            services.AddScoped<IViewProviderManager<AdminIndex>, ViewProviderManager<AdminIndex>>();
            services.AddScoped<IViewProvider<AdminIndex>, AdminIndexViewProvider>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, FeatureSubscriber>();
            services.AddScoped<IBrokerSubscriber, UserSubscriber>();
            services.AddScoped<IBrokerSubscriber, SiteSettingsSubscriber>();
            services.AddScoped<IBrokerSubscriber, EmailSettingsSubscriber>();

            // Configuration
            services.AddTransient<IConfigureOptions<TourOptions>, TourOptionsConfiguration>();

        }

    }

}