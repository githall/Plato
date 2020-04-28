using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.Ratings.Assets;
using PlatoCore.Assets.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using Plato.Entities.Ratings.Handlers;
using Plato.Entities.Ratings.Models;
using Plato.Entities.Ratings.Repositories;
using Plato.Entities.Ratings.Services;
using Plato.Entities.Ratings.Stores;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Entities.Ratings
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
            services.AddScoped<IEntityRatingsRepository<EntityRating>, EntityRatingRepository>();

            // Stores
            services.AddScoped<IEntityRatingsStore<EntityRating>, EntityRatingsStore>();
            services.AddScoped<IEntityRatingsAggregateStore, EntityRatingsAggregateStore>();

            // Managers
            services.AddScoped<IEntityRatingsManager<EntityRating>, EntityRatingsManager>();

            // Register client resources
            services.AddScoped<IAssetProvider, AssetProvider>();            

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "EntityRatingsWebApi",
                areaName: "Plato.Entities.Ratings",
                template: "api/ratings/{controller}/{action}/{id?}",
                defaults: new { controller = "Entity", action = "Get" }
            );

        }

    }

}