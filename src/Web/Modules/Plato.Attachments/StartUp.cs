using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Attachments.Handlers;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using Plato.Attachments.Repositories;
using Plato.Attachments.Stores;
using Plato.Attachments.Assets;
using PlatoCore.Assets.Abstractions;
using Plato.Attachments.Models;
using Plato.Attachments.ViewProviders;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using Plato.Attachments.Navigation;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Attachments
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

            // Register client assets
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // Repositories 
            services.AddScoped<IAttachmentRepository<Attachment>, AttachmentRepository>();

            // Stores
            services.AddScoped<IAttachmentStore<Attachment>, AttachmentStore>();
            services.AddScoped<IAttachmentSettingsStore<AttachmentSettings>, AttachmentSettingsStore>();

            // View providers
            services.AddScoped<IViewProviderManager<AttachmentSetting>, ViewProviderManager<AttachmentSetting>>();
            services.AddScoped<IViewProvider<AttachmentSetting>, AdminViewProvider>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Index
            routes.MapAreaRoute(
                name: "AttachmentsIndex",
                areaName: "Plato.Attachments",
                template: "admin/settings/attachments",
                defaults: new { controller = "Admin", action = "Index" }
            );

            // Edit
            routes.MapAreaRoute(
                name: "AttachmentsEdit",
                areaName: "Plato.Attachments",
                template: "admin/settings/attachments/edit/{id:int}",
                defaults: new { controller = "Admin", action = "Edit" }
            );

            // Display
            routes.MapAreaRoute(
                name: "ServeAttachment",
                areaName: "Plato.Attachments",
                template: "attachment/{id?}",
                defaults: new { controller = "Attachment", action = "Serve" }
            );

            // API
            routes.MapAreaRoute(
                name: "AttachmentWebApi",
                areaName: "Plato.Attachments",
                template: "api/attachments/{controller}/{action}/{id?}",
                defaults: new { controller = "Upload", action = "Index" }
            );


        }

    }

}