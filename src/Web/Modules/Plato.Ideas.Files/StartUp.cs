using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Ideas.Models;
using PlatoCore.Layout.ViewProviders;
using Plato.Ideas.Files.ViewProviders;
using Plato.Ideas.Files.Navigation;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Ideas.Files.Handlers;
using PlatoCore.Features.Abstractions;

namespace Plato.Ideas.Files
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
            services.AddScoped<INavigationProvider, IdeaFooterMenu>();

            // View providers
            services.AddScoped<IViewProviderManager<Idea>, ViewProviderManager<Idea>>();
            services.AddScoped<IViewProvider<Idea>, IdeaViewProvider>();

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
                name: "IdeasFileDownload",
                areaName: "Plato.Ideas.Files",
                template: "ideas/files/d/{id:int}/{alias?}",
                defaults: new { controller = "Home", action = "Download" }
            );

            // Edit
            routes.MapAreaRoute(
                name: "EditIdeaFiles",
                areaName: "Plato.Ideas.Files",
                template: "ideas/files/edit/{opts.guid}/{opts.entityId:int?}",
                defaults: new { controller = "Home", action = "Edit" }
            );

            // Preview
            routes.MapAreaRoute(
                name: "PreviewIdeaFiless",
                areaName: "Plato.Ideas.Files",
                template: "ideas/files/preview/{opts.guid}/{opts.entityId:int?}",
                defaults: new { controller = "Home", action = "Preview" }
            );

            // API
            routes.MapAreaRoute(
                name: "IdeaFilesWebApi",
                areaName: "Plato.Ideas.Files",
                template: "api/ideas/files/{action}/{id:int?}",
                defaults: new { controller = "Api", action = "Index" }
            );

        }

    }

}