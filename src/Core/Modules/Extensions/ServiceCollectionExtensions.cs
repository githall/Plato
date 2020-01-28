using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using PlatoCore.Modules.Abstractions;
using PlatoCore.Modules.Configuration;
using PlatoCore.Modules.Loader;
using PlatoCore.Modules.Locator;
using PlatoCore.Modules.Models;

namespace PlatoCore.Modules.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoModules(
            this IServiceCollection services)
        {

            services.AddSingleton<IConfigureOptions<ModuleOptions>, ModuleOptionsConfigure>();
            services.AddSingleton<IModuleLocator, ModuleLocator>();
            services.AddSingleton<IModuleLoader, ModuleLoader>();
            services.AddSingleton<IModuleManager, ModuleManager>();
            services.AddSingleton<ITypedModuleProvider, TypedModuleProvider>();

            return services;
        }


        //public static void UseModuleStaticFiles(
        //    this IApplicationBuilder app, IHostingEnvironment env)
        //{

        //    var moduleManager = app.ApplicationServices.GetRequiredService<IModuleManager>();
        //    var modules = moduleManager.LoadModulesAsync().Result;
        //    foreach (var moduleEntry in modules)
        //    {

        //        // serve static files within module folders
        //        var contentPath = Path.Combine(
        //            env.ContentRootPath,
        //            moduleEntry.Descriptor.Location,
        //            moduleEntry.Descriptor.Id,
        //            "Content");

        //        if (Directory.Exists(contentPath))
        //        {

        //            app.UseStaticFiles(new StaticFileOptions
        //            {
        //                RequestPath = "/" + moduleEntry.Descriptor.Id.ToLower() + "/content",
        //                FileProvider = new PhysicalFileProvider(contentPath)
        //            });

        //            app.UseStaticFiles(new StaticFileOptions
        //            {
        //                RequestPath = "/" + moduleEntry.Descriptor.Id.ToLower() + "/content",
        //                FileProvider = new EmbeddedFileProvider(moduleEntry.Assembly)
        //            });
                    
        //            if (moduleEntry.ViewsAssembly != null)
        //            {
        //                app.UseStaticFiles(new StaticFileOptions
        //                {
        //                    RequestPath = "/" + moduleEntry.Descriptor.Id.ToLower() + "/content",
        //                    FileProvider = new EmbeddedFileProvider(moduleEntry.ViewsAssembly)
        //                });
        //            }
                 
        //        }

        //    }

        //}

    }
}
