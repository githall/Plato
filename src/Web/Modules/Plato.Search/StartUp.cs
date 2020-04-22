﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Core.Models;
using PlatoCore.Assets.Abstractions;
using PlatoCore.Data.Schemas.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Search.Assets;
using Plato.Search.Commands;
using Plato.Search.Configuration;
using Plato.Search.Models;
using Plato.Search.Navigation;
using Plato.Search.Stores;
using Plato.Search.ViewProviders;
using Plato.Search.Handlers;
using Plato.Search.Repositories;
using Plato.Search.Services;

namespace Plato.Search
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
            
            // Stores
            services.AddScoped<ISearchSettingsStore<SearchSettings>, SearchSettingsStore>();

            // Navigation
            services.AddScoped<INavigationProvider, SearchMenu>();
            services.AddScoped<INavigationProvider, SiteSearchMenu>();
            services.AddScoped<INavigationProvider, SiteMenu>();
            services.AddScoped<INavigationProvider, AdminMenu>();

            // View providers
            services.AddScoped<IViewProviderManager<SearchResult>, ViewProviderManager<SearchResult>>();
            services.AddScoped<IViewProvider<SearchResult>, SearchViewProvider>();
            
            // Admin view providers
            services.AddScoped<IViewProviderManager<SearchSettings>, ViewProviderManager<SearchSettings>>();
            services.AddScoped<IViewProvider<SearchSettings>, AdminViewProvider>();

            // Homepage view providers
            services.AddScoped<IViewProviderManager<HomeIndex>, ViewProviderManager<HomeIndex>>();
            services.AddScoped<IViewProvider<HomeIndex>, HomeViewProvider>();
            
            // Register assets
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Repositories
            services.AddScoped<IFullTextCatalogRepository, FullTextCatalogRepository>();
            services.AddScoped<IFullTextIndexRepository, FullTextIndexRepository>();

            // Stores
            services.AddScoped<IFullTextCatalogStore, FullTextCatalogStore>();
            services.AddScoped<IFullTextIndexStore, FullTextIndexStore>();
            
            // Commands
            services.AddScoped<IFullTextCatalogCommand<SchemaFullTextCatalog>, FullTextCatalogCommand>();
            services.AddScoped<IFullTextIndexCommand<SchemaFullTextIndex>, FullTextIndexCommand>();

            // Services
            services.AddScoped<IFullTextCatalogManager, FullTextCatalogManager>();
        
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();
         
            // Configure site options
            services.AddSingleton<IConfigureOptions<SearchOptions>, SearchOptionsConfiguration>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            
            // Search index

            routes.MapAreaRoute(
                name: "PlatoSearchIndex",
                areaName: "Plato.Search",
                template: "search/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );
            
            // Admin settings

            routes.MapAreaRoute(
                name: "PlatoSearchAdmin",
                areaName: "Plato.Search",
                template: "admin/settings/search",
                defaults: new { controller = "Admin", action = "Index" }
            );

            // Web Api

            routes.MapAreaRoute(
                name: "PlatoSearchWebApi",
                areaName: "Plato.Search",
                template: "api/search/{action}/{id?}",
                defaults: new { controller = "Api", action = "Get" }
            );

        }

    }
}