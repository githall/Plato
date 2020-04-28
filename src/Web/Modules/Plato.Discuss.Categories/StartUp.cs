﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using Plato.Categories.Models;
using Plato.Categories.Repositories;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Categories.Subscribers;
using Plato.Discuss.Categories.Models;
using Plato.Discuss.Categories.Navigation;
using Plato.Discuss.Categories.Services;
using Plato.Discuss.Categories.Subscribers;
using Plato.Discuss.Categories.ViewAdapters;
using Plato.Discuss.Categories.ViewProviders;
using Plato.Discuss.Models;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Stores;
using PlatoCore.Stores.Abstractions.QueryAdapters;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Discuss.Categories
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
            
            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<INavigationProvider, SiteMenu>();

            // Repositories
            services.AddScoped<ICategoryDataRepository<CategoryData>, CategoryDataRepository>();
            services.AddScoped<ICategoryRoleRepository<CategoryRole>, CategoryRoleRepository>();
            services.AddScoped<ICategoryRepository<Category>, CategoryRepository<Category>>();

            // Stores
            services.AddScoped<ICategoryDataStore<CategoryData>, CategoryDataStore>();
            services.AddScoped<ICategoryRoleStore<CategoryRole>, CategoryRoleStore>();
            services.AddScoped<ICategoryStore<Category>, CategoryStore<Category>>();
            services.AddScoped<ICategoryManager<Category>, CategoryManager<Category>>();

            // CategoryService needs to be transient as it contains action
            // delegates that can change state several times per request
            services.AddTransient<ICategoryService<Category>, CategoryService<Category>>();
            
            // Discuss view providers
            services.AddScoped<IViewProviderManager<Topic>, ViewProviderManager<Topic>>();
            services.AddScoped<IViewProvider<Topic>, TopicViewProvider>();
            services.AddScoped<IViewProviderManager<Reply>, ViewProviderManager<Reply>>();
            services.AddScoped<IViewProvider<Reply>, ReplyViewProvider>();

            // Home view provider
            services.AddScoped<IViewProviderManager<Category>, ViewProviderManager<Category>>();
            services.AddScoped<IViewProvider<Category>, CategoryViewProvider>();

            // Admin view providers
            services.AddScoped<IViewProviderManager<CategoryAdmin>, ViewProviderManager<CategoryAdmin>>();
            services.AddScoped<IViewProvider<CategoryAdmin>, AdminViewProvider>();
         
            // Register view adapters
            services.AddScoped<IViewAdapterProvider, TopicListItemViewAdapter>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Topic>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Reply>>();
            services.AddScoped<IBrokerSubscriber, CategorySubscriber<Category>>();

            // Category details updater
            services.AddScoped<ICategoryDetailsUpdater, CategoryDetailsUpdater>();

            // Query adapters
            services.AddScoped<IQueryAdapterManager<Category>, QueryAdapterManager<Category>>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            routes.MapAreaRoute(
                name: "DiscussCategoriesIndex",
                areaName: "Plato.Discuss.Categories",
                template: "discuss/categories/{opts.categoryId:int?}/{opts.alias?}/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

        }

    }

}