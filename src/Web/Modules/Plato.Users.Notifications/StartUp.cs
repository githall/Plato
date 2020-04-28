﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Assets.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Navigation.Abstractions;
using Plato.Users.Notifications.Assets;
using Plato.Users.Notifications.Navigation;
using Plato.Users.Notifications.ViewModels;
using Plato.Users.Notifications.ViewProviders;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Users.Notifications
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

            // Register navigation provider
            services.AddScoped<INavigationProvider, SiteMenu>();
            services.AddScoped<INavigationProvider, EditProfileMenu>();

            // Edit profile view provider
            services.AddScoped<IViewProviderManager<EditNotificationsViewModel>, ViewProviderManager<EditNotificationsViewModel>>();
            services.AddScoped<IViewProvider<EditNotificationsViewModel>, EditProfileViewProvider>();

            // Register assets
            services.AddScoped<IAssetProvider, AssetProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "EditUserNotifications",
                areaName: "Plato.Users.Notifications",
                template: "notifications/edit",
                defaults: new { controller = "Home", action = "Index" }
            );

        }

    }

}