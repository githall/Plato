using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using Plato.Articles.Categories.Navigation;
using Plato.Articles.Categories.Subscribers;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Navigation.Abstractions;
using Plato.Categories.Models;
using Plato.Categories.Repositories;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Articles.Categories.Models;
using Plato.Articles.Categories.ViewAdapters;
using Plato.Articles.Categories.ViewProviders;
using Plato.Articles.Models;
using Plato.Articles.Categories.Services;
using Plato.Categories.Subscribers;
using PlatoCore.Stores;
using PlatoCore.Stores.Abstractions.QueryAdapters;

namespace Plato.Articles.Categories
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
            services.AddScoped<ICategoryService<Category>, CategoryService<Category>>();

            // Articles view providers
            services.AddScoped<IViewProviderManager<Article>, ViewProviderManager<Article>>();
            services.AddScoped<IViewProvider<Article>, ArticleViewProvider>();
            services.AddScoped<IViewProviderManager<Comment>, ViewProviderManager<Comment>>();
            services.AddScoped<IViewProvider<Comment>, CommentViewProvider>();

            // Home view provider
            services.AddScoped<IViewProviderManager<Category>, ViewProviderManager<Category>>();
            services.AddScoped<IViewProvider<Category>, CategoryViewProvider>();

            // Admin view providers
            services.AddScoped<IViewProviderManager<CategoryAdmin>, ViewProviderManager<CategoryAdmin>>();
            services.AddScoped<IViewProvider<CategoryAdmin>, AdminViewProvider>();

            // Register view adapters
            services.AddScoped<IViewAdapterProvider, ArticleListItemViewAdapter>();

            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Article>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Comment>>();
            services.AddScoped<IBrokerSubscriber, CategorySubscriber<Category>>();

            // Channel details updater
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
                name: "ArticleCategoryIndex",
                areaName: "Plato.Articles.Categories",
                template: "articles/categories/{opts.categoryId:int?}/{opts.alias?}/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

        }
    }
}