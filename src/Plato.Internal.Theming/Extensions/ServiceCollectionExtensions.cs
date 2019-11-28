using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Theming.Abstractions;
using Plato.Internal.Theming.Abstractions.Locator;
using Plato.Internal.Theming.Configuration;

namespace Plato.Internal.Theming.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoTheming(
            this IServiceCollection services)
        {

            // Configuration
            services.AddSingleton<IConfigureOptions<ThemeOptions>, ThemeOptionsConfigure>();
            
            // Locater, loader & manager
            services.AddSingleton<IThemeLoader, ThemeLoader>();
            services.AddSingleton<IThemeLocator, ThemeLocator>();
            services.AddSingleton<IThemeFileManager, ThemeFileManager>();

            // Creator & updater
            services.AddScoped<IThemeCreator, ThemeCreator>();
            services.AddSingleton<IThemeUpdater, ThemeUpdater>();

            // Selector
            services.AddScoped<IThemeSelector, ThemeSelector>();

            // Dummy implementations to mimic IThemeManager, until the theming feature is enabled
            services.AddSingleton<ISiteThemeLoader, DummySiteThemeLoader>();
            services.AddSingleton<ISiteThemeFileManager, DummySiteThemeFileManager>();
            
            return services;

        }

    }

}
