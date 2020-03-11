using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Articles.Models;
using PlatoCore.Layout.ViewProviders;
using Plato.Articles.Attachments.ViewProviders;
using Plato.Articles.Attachments.Navigation;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Articles.Attachments.Handlers;
using PlatoCore.Features.Abstractions;

namespace Plato.Articles.Attachments
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
                name: "ArticlesAttachmentDownload",
                areaName: "Plato.Articles.Attachments",
                template: "articles/attachments/d/{id:int}/{alias?}",
                defaults: new { controller = "Home", action = "Download" }
            );

            // Edit
            routes.MapAreaRoute(
                name: "EditArticleAttachments",
                areaName: "Plato.Articles.Attachments",
                template: "articles/attachments/edit/{opts.guid}/{opts.entityId:int?}",
                defaults: new { controller = "Home", action = "Edit" }
            );

            // Preview
            routes.MapAreaRoute(
                name: "PreviewArticleAttachments",
                areaName: "Plato.Articles.Attachments",
                template: "articles/attachments/preview/{opts.guid}/{opts.entityId:int?}",
                defaults: new { controller = "Home", action = "Preview" }
            );

            // API
            routes.MapAreaRoute(
                name: "ArticleAttachmentWebApi",
                areaName: "Plato.Articles.Attachments",
                template: "api/articles/attachments/{action}/{id:int?}",
                defaults: new { controller = "Api", action = "Index" }
            );

        }

    }

}