﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.ActionFilters;
using Plato.Core.Models;
using Plato.Entities.Assets;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Models.Shell;
using Plato.Entities.Handlers;
using Plato.Entities.Models;
using Plato.Entities.Navigation;
using Plato.Entities.Repositories;
using Plato.Entities.Search;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.Subscribers;
using Plato.Entities.ViewProviders;
using PlatoCore.Assets.Abstractions;
using PlatoCore.Data.Migrations.Abstractions;
using PlatoCore.Layout.ActionFilters;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Models.Users;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Search.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores;
using PlatoCore.Stores.Abstractions.FederatedQueries;
using PlatoCore.Stores.Abstractions.QueryAdapters;

namespace Plato.Entities
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

            // Register client resources
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Register navigation provider
            services.AddScoped<INavigationProvider, UserEntitiesMenu>();

            // Repositories
            services.AddScoped<IEntityRepository<Entity>, EntityRepository<Entity>>();
            services.AddScoped<ISimpleEntityRepository<SimpleEntity>, SimpleEntityRepository<SimpleEntity>>();
            services.AddScoped<IEntityDataRepository<IEntityData>, EntityDataRepository>();
            services.AddScoped<IEntityReplyRepository<EntityReply>, EntityReplyRepository<EntityReply>>();
            services.AddScoped<IEntityReplyDataRepository<IEntityReplyData>, EntityReplyDataRepository>();
            services.AddScoped<IEntityUsersRepository, EntityUsersRepository>();
            services.AddScoped<IAggregatedEntityRepository, AggregatedEntityRepository>();
            services.AddScoped<IAggregatedEntityReplyRepository, AggregatedEntityReplyRepository>();
            services.AddScoped<IFeatureEntityCountRepository, FeatureEntityCountRepository>();

            // Stores
            services.AddScoped<IEntityStore<Entity>, EntityStore<Entity>>();
            services.AddScoped<ISimpleEntityStore<SimpleEntity>, SimpleEntityStore<SimpleEntity>>();
            services.AddScoped<IEntityDataStore<IEntityData>, EntityDataStore>();
            services.AddScoped<IEntityDataItemStore<EntityData>, EntityDataItemStore<EntityData>>();
            services.AddScoped<IEntityReplyDataStore<IEntityReplyData>, EntityReplyDataStore>();
            services.AddScoped<IEntityReplyStore<EntityReply>, EntityReplyStore<EntityReply>>();
            services.AddScoped<IEntityUsersStore, EntityUsersStore>();
            services.AddScoped<IFeatureEntityCountStore, FeatureEntityCountStore>();

            // Entity services - transient as they contains action
            // delegates that can change state several times per request
            services.AddTransient<IEntityService<Entity>, EntityService<Entity>>();
            services.AddTransient<ISimpleEntityService<SimpleEntity>, SimpleEntityService<SimpleEntity>>();
            services.AddTransient<IFeatureEntityCountService, FeatureEntityCountService>();

            // Managers
            services.AddScoped<IEntityManager<Entity>, EntityManager<Entity>>();
            services.AddScoped<IEntityReplyManager<EntityReply>, EntityReplyManager<EntityReply>>();

            // Entity subscribers
            services.AddScoped<IBrokerSubscriber, ParseEntityAliasSubscriber>();
            services.AddScoped<IBrokerSubscriber, ParseEntityUrlsSubscriber>();
            services.AddScoped<IBrokerSubscriber, ParseEntityHtmlSubscriber>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<EntityReply>>();

            // Full text index providers
            services.AddScoped<IFullTextIndexProvider, FullTextIndexes>();
            
            // Federated search
            services.AddScoped<IFederatedQueryManager<Entity>, FederatedQueryManager<Entity>>();
            services.AddScoped<IFederatedQueryProvider<Entity>, EntityQueries<Entity>>();
            services.AddScoped<IFederatedQueryManager<SimpleEntity>, FederatedQueryManager<SimpleEntity>>();
            services.AddScoped<IFederatedQueryProvider<SimpleEntity>, SimpleEntityQueries<SimpleEntity>>();
            services.AddScoped<IFederatedQueryManager<FeatureEntityCount>, FederatedQueryManager<FeatureEntityCount>>();
            services.AddScoped<IFederatedQueryProvider<FeatureEntityCount>, FeatureEntityCountQueries<FeatureEntityCount>>();

            // Query adapters
            services.AddScoped<IQueryAdapterManager<Entity>, QueryAdapterManager<Entity>>();
            services.AddScoped<IQueryAdapterManager<SimpleEntity>, QueryAdapterManager<SimpleEntity>>();
            services.AddScoped<IQueryAdapterManager<EntityQueryParams>, QueryAdapterManager<EntityQueryParams>>();
            services.AddScoped<IQueryAdapterManager<FeatureEntityCount>, QueryAdapterManager<FeatureEntityCount>>();

            // Profile view providers
            services.AddScoped<IViewProviderManager<ProfilePage>, ViewProviderManager<ProfilePage>>();
            services.AddScoped<IViewProvider<ProfilePage>, ProfileViewProvider>();

            // User view providers
            services.AddScoped<IViewProviderManager<EntityUserIndex>, ViewProviderManager<EntityUserIndex>>();
            services.AddScoped<IViewProvider<EntityUserIndex>, UserViewProvider>();

            // Homepage view providers
            services.AddScoped<IViewProviderManager<HomeIndex>, ViewProviderManager<HomeIndex>>();
            services.AddScoped<IViewProvider<HomeIndex>, HomeViewProvider>();

            // Action filter
            services.AddScoped<IModularActionFilter, HomeMenuContextualizeFilter>();

            // Migrations
            services.AddSingleton<IMigrationProvider, Migrations>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            // Get Entity
            routes.MapAreaRoute(
                name: "GetEntity",
                areaName: "Plato.Entities",
                template: "e/{opts.id:int}/{opts.alias?}",
                defaults: new { controller = "Home", action = "GetEntity" }
            );
            
            // User Index
            routes.MapAreaRoute(
                name: "EntitiesUser",
                areaName: "Plato.Entities",
                template: "u/{opts.createdByUserId:int}/{opts.alias?}/all/{pager.offset:int?}",
                defaults: new { controller = "User", action = "Index" }
            );

            // Web API
            routes.MapAreaRoute(
                name: "EntitiesWebApi",
                areaName: "Plato.Entities",
                template: "api/entities/{controller}/{action}/{id?}",
                defaults: new { controller = "Entity", action = "Get" }
            );

        }
    }
}