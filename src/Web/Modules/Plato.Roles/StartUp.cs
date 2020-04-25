using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlatoCore.Abstractions.SetUp;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Models.Roles;
using PlatoCore.Models.Shell;
using PlatoCore.Models.Users;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Roles;
using PlatoCore.Navigation.Abstractions;
using Plato.Roles.ViewProviders;
using Plato.Roles.Handlers;
using Plato.Roles.Navigation;
using Plato.Roles.Services;
using PlatoCore.Data.Migrations.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Roles
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
            
            // Replace dummy role stores registered via User StartUp with real implementations
            services.TryAddScoped<IRoleStore<Role>, RoleStore>();
            services.TryAddScoped<IRoleClaimStore<Role>, RoleStore>();

            // Register role manager
            services.TryAddScoped<RoleManager<Role>>();

            // User view providers
            services.AddScoped<IViewProviderManager<User>, ViewProviderManager<User>>();
            services.AddScoped<IViewProvider<User>, UserViewProvider>();

            // Role view provider
            services.AddScoped<IViewProviderManager<Role>, ViewProviderManager<Role>>();
            services.AddScoped<IViewProvider<Role>, AdminViewProvider>();

            // Register navigation providers
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<INavigationProvider, SiteMenu>();

            // Register moderation permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

            // Register default role manager
            services.AddScoped<IDefaultRolesManager, DefaultRolesManager>();

            // Register feature & set-up event handler
            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // Register additional authorization handler for implied permissions
            services.AddScoped<IAuthorizationHandler, RolesPermissionsHandler>();

            // Migrations
            services.AddSingleton<IMigrationProvider, Migrations>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Index
            routes.MapAreaRoute(
                name: "RolesIndex",
                areaName: "Plato.Roles",
                template: "admin/roles",
                defaults: new { controller = "Admin", action = "Index" }
            );

            // Add
            routes.MapAreaRoute(
                name: "RolesCreate",
                areaName: "Plato.Roles",
                template: "admin/roles/add",
                defaults: new { controller = "Admin", action = "Create" }
            );

            // Edit
            routes.MapAreaRoute(
                name: "RolesEdit",
                areaName: "Plato.Roles",
                template: "admin/roles/edit/{id:int}",
                defaults: new { controller = "Admin", action = "Edit" }
            );
        }

    }

}