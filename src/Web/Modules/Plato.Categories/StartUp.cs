﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Categories.Assets;
using Plato.Categories.Handlers;
using Plato.Categories.Models;
using Plato.Categories.Repositories;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Categories.Subscribers;
using PlatoCore.Assets.Abstractions;
using PlatoCore.Data.Migrations.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Stores;
using PlatoCore.Stores.Abstractions.QueryAdapters;

namespace Plato.Categories
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
            
            //// Feature event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Repositories
            services.AddScoped<ICategoryDataRepository<CategoryData>, CategoryDataRepository>();
            services.AddScoped<ICategoryRoleRepository<CategoryRole>, CategoryRoleRepository>();
            services.AddScoped<ICategoryRepository<CategoryBase>, CategoryRepository<CategoryBase>>();
            services.AddScoped<IEntityCategoryRepository<EntityCategory>, EntityCategoryRepository>();

            // Stores
            services.AddScoped<ICategoryStore<CategoryBase>, CategoryStore<CategoryBase>>();
            services.AddScoped<ICategoryDataStore<CategoryData>, CategoryDataStore>();
            services.AddScoped<ICategoryRoleStore<CategoryRole>, CategoryRoleStore>();
            services.AddScoped<IEntityCategoryStore<EntityCategory>, EntityCategoryStore>();

            // Services
            services.AddScoped<ICategoryService<CategoryBase>, CategoryService<CategoryBase>>();

            // Managers
            services.AddScoped<ICategoryManager<CategoryBase>, CategoryManager<CategoryBase>>();
            services.AddScoped<IEntityCategoryManager, EntityCategoryManager>();

            // Client assets
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Subscribers
            services.AddScoped<IBrokerSubscriber, CategorySubscriber<CategoryBase>>();

            // Query adapters
            services.AddScoped<IQueryAdapterManager<CategoryBase>, QueryAdapterManager<CategoryBase>>();
            
            // Migrations
            services.AddSingleton<IMigrationProvider, Migrations>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // wen api routes
            routes.MapAreaRoute(
                name: "CategoriesApi",
                areaName: "Plato.Categories",
                template: "api/categories/{action}/{featureId?}",
                defaults: new { controller = "Categories", action = "Get" }
            );

        }

    }

}