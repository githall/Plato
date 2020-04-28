﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Assets.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Search.Abstractions;
using Plato.Labels.Assets;
using Plato.Labels.Handlers;
using Plato.Labels.Models;
using Plato.Labels.Repositories;
using Plato.Labels.Services;
using Plato.Labels.Stores;
using Plato.Labels.Subscribers;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Labels
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
            
            // Feature event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Repositories
            services.AddScoped<ILabelRepository<LabelBase>, LabelRepository<LabelBase>>();
            services.AddScoped<ILabelDataRepository<LabelData>, LabelDataRepository>();            
            services.AddScoped<IEntityLabelRepository<EntityLabel>, EntityLabelRepository>();

            // Stores
            services.AddScoped<ILabelStore<LabelBase>, LabelStore<LabelBase>>();
            services.AddScoped<ILabelDataStore<LabelData>, LabelDataStore>();            
            services.AddScoped<IEntityLabelStore<EntityLabel>, EntityLabelStore>();

            // Managers
            services.AddScoped<ILabelManager<LabelBase>, LabelManager<LabelBase>>();
            services.AddScoped<IEntityLabelManager<EntityLabel>, EntityLabelManager>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, EntityLabelSubscriber>();
          
            // Client assets
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Label service
            services.AddScoped<ILabelService<LabelBase>, LabelService<LabelBase>>();

            // Full text index providers
            services.AddScoped<IFullTextIndexProvider, FullTextIndexes>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Api routes
            routes.MapAreaRoute(
                name: "LabelsWebApi",
                areaName: "Plato.Labels",
                template: "api/labels/{action}/{id?}",
                defaults: new { controller = "Labels", action = "Get" }
            );

        }

    }

}