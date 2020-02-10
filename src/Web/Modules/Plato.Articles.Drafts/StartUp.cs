using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Articles.Drafts.Handlers;
using Plato.Articles.Drafts.Navigation;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using Plato.Articles.Drafts.ViewProviders;
using Plato.Articles.Models;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Articles.Drafts
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
         
            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Register navigation provider
            services.AddScoped<INavigationProvider, PostMenu>();
                 
            // Register view providers
            services.AddScoped<IViewProviderManager<Article>, ViewProviderManager<Article>>();
            services.AddScoped<IViewProvider<Article>, ArticleViewProvider>();
          
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

        }

    }

}