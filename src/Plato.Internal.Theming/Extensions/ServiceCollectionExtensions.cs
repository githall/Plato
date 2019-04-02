﻿using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Theming.Abstractions;
using Plato.Internal.Theming.Abstractions.Locator;

namespace Plato.Internal.Theming.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoTheming(
            this IServiceCollection services)
        {

            services.AddSingleton<IConfigureOptions<ThemeOptions>, ThemeOptionsConfigure>();
            services.AddSingleton<IThemeLocator, ThemeLocator>();

            services.AddSingleton<IThemeManager, ThemeManager>();
            services.AddSingleton<IThemeFileManager, ThemeFileManager>();
            services.AddSingleton<IThemeDescriptorUpdater, ThemeDescriptorUpdater>();
         
            services.AddSingleton<ISiteThemeManager, DummySiteThemeManager>();
            services.AddSingleton<ISiteThemeFileManager, DummySiteThemeFileManager>();


            return services;
        }

        public static void UseThemeStaticFiles(
            this IApplicationBuilder app, 
            IHostingEnvironment env)
        {

            // Add default themes
            var options = app.ApplicationServices.GetRequiredService<IOptions<ThemeOptions>>();
            if (options != null)
            {
                var contentPath = Path.Combine(
                    env.ContentRootPath,
                    options.Value.VirtualPathToThemesFolder);

                app.UseStaticFiles(new StaticFileOptions
                {
                    RequestPath = "/" + options.Value.VirtualPathToThemesFolder,
                    FileProvider = new PhysicalFileProvider(contentPath)
                });
            }

            // Add sites folder
            var sitesFolder = app.ApplicationServices.GetRequiredService<ISitesFolder>();
            if (sitesFolder != null)
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    RequestPath = "/sites",
                    FileProvider = new PhysicalFileProvider(sitesFolder.RootPath)
                });
            }

        }
        
    }

}
