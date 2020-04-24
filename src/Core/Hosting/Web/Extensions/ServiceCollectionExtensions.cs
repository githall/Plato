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
using PlatoCore.Repositories.Extensions;
using PlatoCore.Shell.Extensions;
using PlatoCore.Stores.Extensions;
using PlatoCore.Cache.Extensions;
using PlatoCore.Features.Extensions;
using PlatoCore.Layout.Extensions;
using PlatoCore.Security.Extensions;
using PlatoCore.Logging.Extensions;
using PlatoCore.Messaging.Extensions;
using PlatoCore.Assets.Extensions;
using PlatoCore.Badges.Extensions;
using PlatoCore.Drawing.Extensions;
using PlatoCore.Localization.Extensions;
using PlatoCore.Navigation.Extensions;
using PlatoCore.Net.Extensions;
using PlatoCore.Notifications.Extensions;
using PlatoCore.Reputations.Extensions;
using PlatoCore.Scripting.Extensions;
using PlatoCore.Search.Extensions;
using PlatoCore.Tasks.Extensions;
using PlatoCore.Text.Extensions;
using PlatoCore.Theming.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using PlatoCore.Abstractions.Routing;
using PlatoCore.Abstractions.Settings;
using PlatoCore.FileSystem;
using PlatoCore.FileSystem.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Hosting.Web.Configuration;
using PlatoCore.Hosting.Web.Routing;
using PlatoCore.Layout.LocationExpanders;
using PlatoCore.Layout.ViewFeatures;
using PlatoCore.Modules.FileProviders;
using PlatoCore.Models.Shell;
using PlatoCore.Stores.Abstractions.Shell;
using PlatoCore.Features.Abstractions;
using PlatoCore.Features;
using PlatoCore.Stores.Shell;
using Microsoft.AspNetCore.Routing;

namespace PlatoCore.Hosting.Web.Extensions
{

    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlato(this IServiceCollection services)
        {
            services.ConfigureShell("Sites");            
            services.AddHttpContextAccessor();     
            services.AddPlatoDataProtection();
            services.AddPlatoHost();
            return services;
        }

        public static IServiceCollection AddPlatoHost(this IServiceCollection services)
        {

            return services.AddHPlatoTennetHost(internalServices =>
            {

                // infrastructure
                // --------

                internalServices.AddLogging(loggingBuilder =>
                {                    
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddDebug();
                });

                internalServices.AddOptions();
                internalServices.AddLocalization(options => options.ResourcesPath = "Resources");

                // Plato
                // --------

                internalServices.AddSingleton<IPlatoHostEnvironment, WebHostEnvironment>();
                internalServices.AddSingleton<IPlatoFileSystem, HostedFileSystem>();
                internalServices.AddPlatoOptions();
                internalServices.AddPlatoContextAccessor();
              
                internalServices.AddPlatoLocalization();
                internalServices.AddPlatoCaching();
                internalServices.AddPlatoText();
                internalServices.AddPlatoNotifications();
                internalServices.AddPlatoModules();
                internalServices.AddPlatoTheming();
                internalServices.AddPlatoViewFeature();
                internalServices.AddPlatoViewLocalization();
                //internalServices.AddPlatoNavigation();
                internalServices.AddPlatoAssets();
                //internalServices.AddPlatoNet();
                internalServices.AddPlatoScripting();
                //internalServices.AddPlatoShellFeatures();
                //internalServices.AddPlatoMessaging();
                internalServices.AddPlatoLogging();
                internalServices.AddPlatoDbContext();
                //internalServices.AddPlatoRepositories();
                //internalServices.AddPlatoStores();
                //internalServices.AddPlatoReputations();
                internalServices.AddPlatoBadges();
                internalServices.AddPlatoDrawing();
                //internalServices.AddPlatoTasks();
                internalServices.AddPlatoSearch();               
                internalServices.AddPlatoMvc();
                internalServices.AddPlatoRouting();
            });

        }

        public static IServiceCollection AddHPlatoTennetHost(this IServiceCollection services, Action<IServiceCollection> configure)
        {

            // Dummy implementations to pass .NET Core dependency injection checks
            // These are replaced for each tenant via the ShellContainerFactory
            //services.AddScoped<IShellSettings, ShellSettings>();
            //services.AddScoped<IShellDescriptorStore, ShellDescriptorStore>();
            //services.AddScoped<IShellDescriptorManager, ShellDescriptorManager>();

            // Add host
            services.AddPlatoDefaultHost();

            // Add shell
            services.AddPlatoShell();

            // Add security
            services.AddPlatoSecurity();

            // Let the host change the default tenant behavior and set of features
            configure?.Invoke(services);

            // Register the list of services to be resolved later via ShellContainerFactory
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