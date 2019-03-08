﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.Reactions.Assets;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Reactions.Handlers;
using Plato.Reactions.Models;
using Plato.Reactions.Repositories;
using Plato.Reactions.Services;
using Plato.Reactions.Stores;

namespace Plato.Entities.Reactions
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

            // Repositories
            services.AddScoped<IEntityReactionsRepository<EntityReaction>, EntityReactionRepository>();

            // Stores
            services.AddScoped<IEntityReactionsStore<EntityReaction>, EntityReactionsStore>();
            services.AddScoped<ISimpleReactionsStore, SimpleReactionsStore>();

            // Services
            services.AddScoped<IReactionsManager<Reaction>, ReactionsManager<Reaction>>();

            // Managers
            services.AddScoped<IEntityReactionsManager<EntityReaction>, EntityReactionsesManager>();

            // Default Reaction Providers
            services.AddScoped<IReactionsProvider<Reaction>, DefaultReactions>();
           
            // Register client resources
            services.AddScoped<IAssetProvider, AssetProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "EntityReactionsWebApi",
                areaName: "Plato.Entities.Reactions",
                template: "api/reactions/{controller}/{action}/{id?}",
                defaults: new { controller = "Entity", action = "Get" }
            );

        }

    }

}