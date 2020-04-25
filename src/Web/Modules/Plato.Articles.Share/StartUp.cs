using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Articles.Models;
using PlatoCore.Models.Shell;
using Plato.Articles.Share.Navigation;
using PlatoCore.Features.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Articles.Share.Handlers;
using Plato.Articles.Share.ViewProviders;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Articles.Share
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

            // Navigation providers
            services.AddScoped<INavigationProvider, ArticleMenu>();
            services.AddScoped<INavigationProvider, ArticleCommentMenu>();
            
            // View providers
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

            routes.MapAreaRoute(
                name: "ArticlesShare",
                areaName: "Plato.Articles.Share",
                template: "articles/a/share/{opts.id}/{opts.alias}/{opts.replyId?}",
                defaults: new { controller = "Home", action = "Index" }
            );            

        }

    }

}