using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Articles.Models;
using PlatoCore.Layout.ViewProviders;
using Plato.Articles.Files.ViewProviders;
using Plato.Articles.Files.Navigation;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Articles.Files.Handlers;
using PlatoCore.Features.Abstractions;

namespace Plato.Articles.Files
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
            services.AddScoped<INavigationProvider, ArticleFooterMenu>();

            // View providers
            services.AddScoped<IViewProviderManager<Article>, ViewProviderManager<Article>>();
            services.AddScoped<IViewProvider<Article>, ArticleViewProvider>();

            // Permissionss
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Download
            routes.MapAreaRoute(
                name: "ArticlesFileDownload",
                areaName: "Plato.Articles.Files",
                template: "articles/files/d/{id:int}/{alias?}",
                defaults: new { controller = "Home", action = "Download" }
            );

            // Edit
            routes.MapAreaRoute(
                name: "EditArticleFiles",
                areaName: "Plato.Articles.Files",
                template: "articles/files/edit/{opts.guid}/{opts.entityId:int?}",
                defaults: new { controller = "Home", action = "Edit" }
            );

            // Preview
            routes.MapAreaRoute(
                name: "PreviewArticleFiless",
                areaName: "Plato.Articles.Files",
                template: "articles/files/preview/{opts.guid}/{opts.entityId:int?}",
                defaults: new { controller = "Home", action = "Preview" }
            );

            // API
            routes.MapAreaRoute(
                name: "ArticleFilesWebApi",
                areaName: "Plato.Articles.Files",
                template: "api/articles/files/{action}/{id:int?}",
                defaults: new { controller = "Api", action = "Index" }
            );

        }

    }

}