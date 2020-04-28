﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Discuss.History.Navigation;
using Plato.Discuss.Models;
using PlatoCore.Models.Shell;
using PlatoCore.Messaging.Abstractions;
using Plato.Entities.History.Subscribers;
using PlatoCore.Features.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Discuss.History.Handlers;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Discuss.History
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
            services.AddScoped<INavigationProvider, TopicMenu>();
            services.AddScoped<INavigationProvider, TopicReplyMenu>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Topic>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Reply>>();
          
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            routes.MapAreaRoute(
                name: "DiscussHistory",
                areaName: "Plato.Discuss.History",
                template: "discuss/history/{id:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "DiscussRollbackHistory",
                areaName: "Plato.Discuss.History",
                template: "discuss/history/rollback",
                defaults: new { controller = "Home", action = "Rollback" }
            );

            routes.MapAreaRoute(
                name: "DiscussDeleteHistory",
                areaName: "Plato.Discuss.History",
                template: "discuss/history/delete",
                defaults: new { controller = "Home", action = "Delete" }
            );

        }

    }

}