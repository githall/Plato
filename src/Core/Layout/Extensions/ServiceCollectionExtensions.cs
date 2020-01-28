using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using PlatoCore.Layout.ActionFilters;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.Localizers;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.TagHelpers;
using PlatoCore.Layout.Theming;
using PlatoCore.Layout.Titles;
using PlatoCore.Layout.ViewAdapters;
using PlatoCore.Layout.Views;

namespace PlatoCore.Layout.Extensions
{

    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoViewAdapters(
            this IServiceCollection services)
        {
            services.TryAddScoped<IViewAdapterManager, ViewAdapterManager>();
            return services;
        }

        public static IServiceCollection AddPlatoViewFeature(
            this IServiceCollection services)
        {

            // Layout updater
            services.AddSingleton<ILayoutUpdater, LayoutUpdater>();

            // Views need to be scoped so new instances are created for each request
            services.AddScoped<IViewFactory, ViewFactory>(); 
            services.AddScoped<IPartialInvoker, PartialInvoker>();
            services.AddScoped<IViewInvoker, ViewInvoker>();
            services.AddScoped<IViewHelperFactory, ViewDisplayHelperFactory>();

            // Add page title builder
            services.AddScoped<IPageTitleBuilder, PageTitleBuilder>();

            // Add theming conventions - configures theme layout based on controller prefix
            services.AddSingleton<IApplicationFeatureProvider<ViewsFeature>, ThemingViewsFeatureProvider>();

            // Action filters
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(typeof(ModelBinderAccessorFilter));
                options.Filters.Add(typeof(AlertFilter));
                options.Filters.Add(typeof(ModularFilter));
            });

            // Model binding model accessor
            services.AddScoped<IUpdateModelAccessor, LocalModelBinderAccessor>();

            // Alerter
            services.AddScoped<IAlerter, Alerter>();



            services.AddTagHelpers<AssetsTagHelper>();
            services.AddTagHelpers<CardTagHelper>();


            return services;

        }

        public static IServiceCollection AddTagHelpers<T>(this IServiceCollection services)
        {
            return services
                .AddTransient<ITagHelpersProvider>(sp => new TagHelpersProvider<T>())
                .AddTransient(typeof(T));
        }

        public static IServiceCollection AddPlatoViewLocalization(
            this IServiceCollection services)
        {

            // Localization
            services.AddScoped<IStringLocalizer, LocaleStringLocalizer>();
            services.AddScoped<IHtmlLocalizer, LocaleHtmlLocalizer>();

            // View localization
            services.AddScoped<IViewLocalizer, LocaleViewLocalizer>();

            return services;

        }

    }


    // ---------

    public interface ITagHelpersProvider
    {
        IEnumerable<Type> GetTypes();
    }

    public class AssemblyTagHelpersProvider : ITagHelpersProvider
    {
        private readonly Assembly _assembly;

        public AssemblyTagHelpersProvider(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            _assembly = assembly;
        }

        public IEnumerable<Type> GetTypes() => _assembly.ExportedTypes.Where(t => t.IsSubclassOf(typeof(TagHelper)));

    }

    public class TagHelpersProvider<T> : ITagHelpersProvider
    {
        public IEnumerable<Type> GetTypes() => new Type[] { typeof(T) };
    }


}
