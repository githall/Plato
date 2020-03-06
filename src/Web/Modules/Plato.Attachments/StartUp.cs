﻿using System;
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

            // Configuration
            services.AddSingleton<IConfigureOptions<AttachmentSettings>, AttachmentSettingsConfiguration>();

            // View providers
            services.AddScoped<IViewProviderManager<AttachmentSetting>, ViewProviderManager<AttachmentSetting>>();
            services.AddScoped<IViewProvider<AttachmentSetting>, AttachmentSettingsViewProvider>();
            services.AddScoped<IViewProviderManager<AttachmentIndex>, ViewProviderManager<AttachmentIndex>>();
            services.AddScoped<IViewProvider<AttachmentIndex>, AttachmentIndexViewProvider>();

            // Permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

            // Services
            services.AddScoped<IAttachmentOptionsFactory, AttachmentOptionsFactory>();

            // Action filters
            services.AddScoped<IModularActionFilter, AttachmentClientOptionsFilter>();

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
                template: "admin/attachments",
                defaults: new { controller = "Admin", action = "Index" }
            );

            // Settings
            routes.MapAreaRoute(
                name: "AttachmentsSettings",
                areaName: "Plato.Attachments",
                template: "admin/attachments/settings",
                defaults: new { controller = "Admin", action = "Settings" }
            );

            // EditSettings
            routes.MapAreaRoute(
                name: "AttachmentsEditSettings",
                areaName: "Plato.Attachments",
                template: "admin/attachments/settings/{id:int}",
                defaults: new { controller = "Admin", action = "EditSettings" }
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