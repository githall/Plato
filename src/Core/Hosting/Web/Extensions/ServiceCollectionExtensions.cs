using System;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlatoCore.Data.Extensions;
using PlatoCore.Hosting.Extensions;
using PlatoCore.Modules.Extensions;
using PlatoCore.Shell.Extensions;
using PlatoCore.Cache.Extensions;
using PlatoCore.Layout.Extensions;
using PlatoCore.Security.Extensions;
using PlatoCore.Logging.Extensions;
using PlatoCore.Assets.Extensions;
using PlatoCore.Badges.Extensions;
using PlatoCore.Drawing.Extensions;
using PlatoCore.Localization.Extensions;
using PlatoCore.Notifications.Extensions;
using PlatoCore.Scripting.Extensions;
using PlatoCore.Search.Extensions;
using PlatoCore.Text.Extensions;
using PlatoCore.Theming.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using PlatoCore.Abstractions.Routing;
using PlatoCore.Abstractions.Settings;
using PlatoCore.FileSystem;
using PlatoCore.FileSystem.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Hosting.Web.Configuration;
using PlatoCore.Hosting.Web.Routing;
using PlatoCore.Layout.LocationExpanders;
using PlatoCore.Layout.ViewFeatures;
using PlatoCore.Modules.FileProviders;
using PlatoCore.Hosting.Abstractions;

namespace PlatoCore.Hosting.Web.Extensions
{

    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlato(this IServiceCollection services)
        {
            services.ConfigureShell("Sites");            
            services.AddHttpContextAccessor();     
            services.AddPlatoDataProtection();
            services.AddPlatoInternal();
            return services;
        }

        private static IServiceCollection AddPlatoInternal(this IServiceCollection services)
        {

            return services.AddHPlatoCore(tenantServices =>
            {

                // Tenant Infrastructure
                // --------

                tenantServices.AddLogging(loggingBuilder =>
                {                    
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddDebug();
                });

                tenantServices.AddOptions();
                tenantServices.AddLocalization(options => options.ResourcesPath = "Resources");

                // Tenant core services
                // --------

                tenantServices.AddSingleton<IPlatoHostEnvironment, WebHostEnvironment>();
                tenantServices.AddSingleton<IPlatoFileSystem, HostedFileSystem>();
                tenantServices.AddPlatoOptions();
                tenantServices.AddPlatoContextAccessor();              
                tenantServices.AddPlatoLocalization();
                tenantServices.AddPlatoCaching();
                tenantServices.AddPlatoText();
                tenantServices.AddPlatoNotifications();
                tenantServices.AddPlatoModules();
                tenantServices.AddPlatoTheming();
                tenantServices.AddPlatoViewFeature();
                tenantServices.AddPlatoViewLocalization();
                tenantServices.AddPlatoAssets();
                tenantServices.AddPlatoScripting();           
                tenantServices.AddPlatoLogging();
                tenantServices.AddPlatoDbContext();                
                tenantServices.AddPlatoBadges();
                tenantServices.AddPlatoDrawing();                
                tenantServices.AddPlatoSearch();               
                tenantServices.AddPlatoMvc();
                tenantServices.AddPlatoRouting();
            });

        }

        public static IServiceCollection AddHPlatoCore(this IServiceCollection services, Action<IServiceCollection> configure)
        {

            // Add host
            services.AddPlatoDefaultHost();

            // Add default shell
            services.AddPlatoShell();

            // Add security
            services.AddPlatoSecurity();

            // Let the host change the default tenant behavior and set of features
            configure?.Invoke(services);

            // Register the list of tenant services to be resolved later via ShellContainerFactory
            services.AddSingleton(_ => services);

            return services;

        }

        public static IServiceCollection AddPlatoOptions(this IServiceCollection services)
        {

            services.TryAddEnumerable(new[] 
            {
                ServiceDescriptor.Transient<IConfigureOptions<PlatoOptions>, PlatoOptionsConfiguration>(),
                ServiceDescriptor.Transient<IConfigureOptions<AntiforgeryOptions>, AntiForgeryOptionsConfiguration>()
            });

            return services;

        }

        public static IServiceCollection AddPlatoMvc(this IServiceCollection services)
        {

            // Configure site options
            services.Configure<SiteOptions>(options =>
            {
                options.SiteName = "Plato";
            });

            // Add MVC core services
            // --------------

            // Razor & Views
            var builder = services
                .AddMvcCore()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddViews()
                .AddCors()
                .AddCacheTagHelper()
                .AddRazorViewEngine()
                .AddNewtonsoftJson()
                .AddRazorPages()
                .AddRazorRuntimeCompilation();

            services.AddControllers(options =>
            {
                options.EnableEndpointRouting = false;
            });

            // Add modular application parts
            services.AddPlatoModularAppParts(builder.PartManager);

            // View adapters
            services.AddPlatoViewAdapters();

            // Add module MVC
            services.AddPlatoModularMvc();

            // Custom view model validation
            services.AddPlatoModelValidation();                       

            return services;

        }

        public static IServiceCollection AddPlatoModularAppParts(this IServiceCollection services, ApplicationPartManager partManager)
        {
            var serviceProvider = services.BuildServiceProvider();
            partManager.ApplicationParts.Insert(0, new ModularFeatureApplicationPart(serviceProvider));
            partManager.FeatureProviders.Add(new ModuleViewFeatureProvider(serviceProvider));
            return services;
        }

        public static IServiceCollection AddPlatoModularMvc(this IServiceCollection services)
        {

            // Location expander
            services.AddScoped<IViewLocationExpanderProvider, ModularViewLocationExpander>();
            services.AddScoped<IViewLocationExpanderProvider, AreaViewLocationExpander>();

            // Configure razor options
            services.Configure<RazorViewEngineOptions>(options =>
            {
                // Add composite view location expander
                options.ViewLocationExpanders.Add(new CompositeViewLocationExpander());
            });

            // Configure runtime compilation file providers
            services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
            {
                options.FileProviders.Insert(0, new ModuleViewFileProvider(services.BuildServiceProvider()));
            });

            // Implement our own conventions to automatically add [areas] route attributes to controllers
            // https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/application-model?view=aspnetcore-2.1
            services.TryAddEnumerable(ServiceDescriptor
                .Transient<IApplicationModelProvider, ModuleApplicationModelProvider>());

            return services;

        }

        public static IServiceCollection AddPlatoRouting(this IServiceCollection services)
        {

            // Add home route manager
            services.AddScoped<IHomeRouteManager, HomeRouteManager>();

            // Add default router (required by PlatoRouterMiddleware)            
            services.AddSingleton<IPlatoRouter, PlatoRouter>();

            return services;

        }

        public static IServiceCollection AddPlatoContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();       
            return services;
        }

    }

}