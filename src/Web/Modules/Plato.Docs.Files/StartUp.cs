using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Docs.Models;
using PlatoCore.Layout.ViewProviders;
using Plato.Docs.Files.ViewProviders;
using Plato.Docs.Files.Navigation;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Docs.Files.Handlers;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Docs.Files
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
            services.AddScoped<INavigationProvider, DocFooterMenu>();

            // View providers
            services.AddScoped<IViewProviderManager<Doc>, ViewProviderManager<Doc>>();
            services.AddScoped<IViewProvider<Doc>, DocViewProvider>();

            // Permissions
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Download
            routes.MapAreaRoute(
                name: "DocsFileDownload",
                areaName: "Plato.Docs.Files",
                template: "docs/files/d/{id:int}/{alias?}",
                defaults: new { controller = "Home", action = "Download" }
            );

            // Edit
            routes.MapAreaRoute(
                name: "EditDocFiles",
                areaName: "Plato.Docs.Files",
                template: "docs/files/edit/{opts.guid}/{opts.entityId:int?}",
                defaults: new { controller = "Home", action = "Edit" }
            );

            // Preview
            routes.MapAreaRoute(
                name: "PreviewDocFiless",
                areaName: "Plato.Docs.Files",
                template: "docs/files/preview/{opts.guid}/{opts.entityId:int?}",
                defaults: new { controller = "Home", action = "Preview" }
            );

            // API
            routes.MapAreaRoute(
                name: "DocFilesWebApi",
                areaName: "Plato.Docs.Files",
                template: "api/docs/files/{action}/{id:int?}",
                defaults: new { controller = "Api", action = "Index" }
            );

        }

    }

}