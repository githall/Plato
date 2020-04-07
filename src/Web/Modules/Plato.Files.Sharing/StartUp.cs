using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Files.Sharing.ViewProviders;
using PlatoCore.Layout.ViewProviders;
using Plato.Files.Models;
using Plato.Files.Sharing.Handlers;
using Plato.Files.Sharing.Repositories;
using Plato.Files.Sharing.Models;
using Plato.Files.Sharing.Stores;
using Plato.Files.Sharing.Services;

namespace Plato.Files.Sharing
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

            // Data access
            services.AddScoped<IFileInviteRepository<FileInvite>, FileInviteRepository>();
            services.AddScoped<IFileInviteStore<FileInvite>, FileInviteStore>();

            // View providers     
            services.AddScoped<IViewProviderManager<File>, ViewProviderManager<File>>();
            services.AddScoped<IViewProvider<File>, AdminViewProvider>();

            // Services
            services.AddScoped<IEmailFileInviteService, EmailFileInviteService>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Home

            routes.MapAreaRoute(
                name: "ShareFileHomeIndex",
                areaName: "Plato.Files.Sharing",
                template: "files/d/{id:int}/{token}",
                defaults: new { controller = "Home", action = "Index" }
            );

            // Admin

            routes.MapAreaRoute(
                name: "ShareFileAdminIndex",
                areaName: "Plato.Files.Sharing",
                template: "admin/files/share/{id:int}",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }

    }

}