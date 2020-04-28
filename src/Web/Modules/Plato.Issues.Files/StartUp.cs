using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Issues.Models;
using PlatoCore.Layout.ViewProviders;
using Plato.Issues.Files.ViewProviders;
using Plato.Issues.Files.Navigation;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Issues.Files.Handlers;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Issues.Files
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
            services.AddScoped<INavigationProvider, IssueFooterMenu>();

            // View providers
            services.AddScoped<IViewProviderManager<Issue>, ViewProviderManager<Issue>>();
            services.AddScoped<IViewProvider<Issue>, IssueViewProvider>();

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
                name: "IssuesFileDownload",
                areaName: "Plato.Issues.Files",
                template: "issues/files/d/{id:int}/{alias?}",
                defaults: new { controller = "Home", action = "Download" }
            );

            // Edit
            routes.MapAreaRoute(
                name: "EditIssueFiles",
                areaName: "Plato.Issues.Files",
                template: "issues/files/edit/{opts.guid}/{opts.entityId:int?}",
                defaults: new { controller = "Home", action = "Edit" }
            );

            // Preview
            routes.MapAreaRoute(
                name: "PreviewIssueFiless",
                areaName: "Plato.Issues.Files",
                template: "issues/files/preview/{opts.guid}/{opts.entityId:int?}",
                defaults: new { controller = "Home", action = "Preview" }
            );

            // API
            routes.MapAreaRoute(
                name: "IssuesFilesWebApi",
                areaName: "Plato.Issues.Files",
                template: "api/issues/files/{action}/{id:int?}",
                defaults: new { controller = "Api", action = "Index" }
            );

        }

    }

}