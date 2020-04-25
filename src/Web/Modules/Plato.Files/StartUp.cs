using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Files.Handlers;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using Plato.Files.Repositories;
using Plato.Files.Stores;
using Plato.Files.Assets;
using PlatoCore.Assets.Abstractions;
using Plato.Files.Models;
using Plato.Files.ViewProviders;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using Plato.Files.Navigation;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Files.Configuration;
using Microsoft.Extensions.Options;
using Plato.Files.Services;
using Plato.Files.ActionFilters;
using PlatoCore.Layout.ActionFilters;
using PlatoCore.Search.Abstractions;
using PlatoCore.Stores.Abstractions.FederatedQueries;
using Plato.Files.Search;
using PlatoCore.Stores;
using PlatoCore.Stores.Abstractions.QueryAdapters;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Files
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

            // Register client assets
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // Repositories 
            services.AddScoped<IFileRepository<File>, FileRepository>();
            services.AddScoped<IFileInfoRepository<FileInfo>, FileInfoRepository>();

            // Stores
            services.AddScoped<IFileStore<File>, FileStore>();
            services.AddScoped<IFileSettingsStore<FileSettings>, FileSettingsStore>();
            services.AddScoped<IFileInfoStore<FileInfo>, FileInfoStore>();

            // Federated queries
            services.AddScoped<IFederatedQueryManager<File>, FederatedQueryManager<File>>();
            services.AddScoped<IFederatedQueryProvider<File>, FileQueries<File>>();

            // Query adapters
            services.AddScoped<IQueryAdapterManager<File>, QueryAdapterManager<File>>();

            // Configuration
            services.AddSingleton<IConfigureOptions<FileSettings>, FileSettingsConfiguration>();

            // View providers
            services.AddScoped<IViewProviderManager<FileSetting>, ViewProviderManager<FileSetting>>();
            services.AddScoped<IViewProvider<FileSetting>, FileSettingsViewProvider>();
            services.AddScoped<IViewProviderManager<File>, ViewProviderManager<File>>();
            services.AddScoped<IViewProvider<File>, AdminViewProvider>();

            // Permissions
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

            // Services
            services.AddScoped<IFileOptionsFactory, FileOptionsFactory>();
            services.AddScoped<IFileGuidFactory, FileGuidFactory>();
            services.AddScoped<IFileValidator, FileValidator>();
            services.AddScoped<IFileViewIncrementer<File>, FileViewIncrementer>();
            services.AddScoped<IFileManager, FileManager>();

            // Action filters
            services.AddScoped<IModularActionFilter, FileClientOptionsFilter>();

            // Full text index providers
            services.AddScoped<IFullTextIndexProvider, FullTextIndexes>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Index
            routes.MapAreaRoute(
                name: "FilesIndex",
                areaName: "Plato.Files",
                template: "admin/files/{pager.offset:int?}",
                defaults: new { controller = "Admin", action = "Index" }
            );

            // Edit File
            routes.MapAreaRoute(
                name: "FilesEdit",
                areaName: "Plato.Files",
                template: "admin/files/d/{id:int}/{alias?}",
                defaults: new { controller = "Admin", action = "Edit" }
            );

            // Settings
            routes.MapAreaRoute(
                name: "FilesSettings",
                areaName: "Plato.Files",
                template: "admin/files/settings",
                defaults: new { controller = "Admin", action = "Settings" }
            );

            // Edit Settings
            routes.MapAreaRoute(
                name: "FilesEditSettings",
                areaName: "Plato.Files",
                template: "admin/files/settings/{id:int}",
                defaults: new { controller = "Admin", action = "EditSettings" }
            );

            // API
            routes.MapAreaRoute(
                name: "FilesWebApi",
                areaName: "Plato.Files",
                template: "api/files/{action}",
                defaults: new { controller = "Api", action = "Index" }
            );

        }

    }

}