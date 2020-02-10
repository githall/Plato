using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Navigation.Abstractions;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.StopForumSpam.Stores;
using Plato.StopForumSpam.Navigation;
using Plato.StopForumSpam.ViewProviders;

namespace Plato.StopForumSpam
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
            
            // Operations type manager
            services.AddScoped<ISpamOperationManager<SpamOperation>, SpamOperationManager<SpamOperation>>();

            // Spam level checker
            services.AddScoped<ISpamChecker, SpamChecker>();
            
            // Settings store
            services.AddScoped<ISpamSettingsStore<SpamSettings>, SpamSettingsStore>();

            // Admin view provider
            services.AddScoped<IViewProviderManager<SpamSettings>, ViewProviderManager<SpamSettings>>();
            services.AddScoped<IViewProvider<SpamSettings>, AdminViewProvider>();

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            routes.MapAreaRoute(
                name: "PlatoStopForumSpamAdmin",
                areaName: "Plato.StopForumSpam",
                template: "admin/settings/sfs",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }
    }
}