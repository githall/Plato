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
using Plato.Attachments.Configuration;
using Microsoft.Extensions.Options;
using Plato.Attachments.Services;
using Plato.Attachments.ActionFilters;
using PlatoCore.Layout.ActionFilters;
using PlatoCore.Search.Abstractions;

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
            services.AddScoped<IAttachmentInfoRepository<AttachmentInfo>, AttachmentInfoRepository>();

            // Stores
            services.AddScoped<IAttachmentStore<Attachment>, AttachmentStore>();
            services.AddScoped<IAttachmentSettingsStore<AttachmentSettings>, AttachmentSettingsStore>();
            services.AddScoped<IAttachmentInfoStore<AttachmentInfo>, AttachmentInfoStore>();

            // Configuration
            services.AddSingleton<IConfigureOptions<AttachmentSettings>, AttachmentSettingsConfiguration>();

            // View providers
            services.AddScoped<IViewProviderManager<AttachmentSetting>, ViewProviderManager<AttachmentSetting>>();
            services.AddScoped<IViewProvider<AttachmentSetting>, AttachmentSettingsViewProvider>();
            services.AddScoped<IViewProviderManager<AttachmentIndex>, ViewProviderManager<AttachmentIndex>>();
            services.AddScoped<IViewProvider<AttachmentIndex>, AttachmentIndexViewProvider>();

            // Permissionss
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

            // Services
            services.AddScoped<IAttachmentOptionsFactory, AttachmentOptionsFactory>();
            services.AddScoped<IAttachmentGuidFactory, AttachmentGuidFactory>();
            services.AddScoped<IAttachmentValidator, AttachmentValidator>();
            services.AddScoped<IAttachmentViewIncrementer<Attachment>, AttachmentViewIncrementer>();

            // Action filters
            services.AddScoped<IModularActionFilter, AttachmentClientOptionsFilter>();

            // Full text index providers
            services.AddScoped<IFullTextIndexProvider, FullTextIndexes>();

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
                template: "admin/attachments/{pager.offset:int?}",
                defaults: new { controller = "Admin", action = "Index" }
            );

            // Settings
            routes.MapAreaRoute(
                name: "AttachmentsSettings",
                areaName: "Plato.Attachments",
                template: "admin/attachments/settings",
                defaults: new { controller = "Admin", action = "Settings" }
            );

            // Edit Settings
            routes.MapAreaRoute(
                name: "AttachmentsEditSettings",
                areaName: "Plato.Attachments",
                template: "admin/attachments/settings/{id:int}",
                defaults: new { controller = "Admin", action = "EditSettings" }
            );

        }

    }

}