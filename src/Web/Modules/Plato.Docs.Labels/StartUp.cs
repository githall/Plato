using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using PlatoCore.Navigation.Abstractions;
using Plato.Labels.Repositories;
using Plato.Labels.Services;
using Plato.Labels.Stores;
using Plato.Docs.Models;
using Plato.Docs.Labels.Navigation;
using Plato.Docs.Labels.Models;
using Plato.Docs.Labels.ViewAdapters;
using Plato.Docs.Labels.ViewProviders;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Docs.Labels
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

            // Data stores
            services.AddScoped<ILabelRepository<Label>, LabelRepository<Label>>();
            services.AddScoped<ILabelStore<Label>, LabelStore<Label>>();
            services.AddScoped<ILabelManager<Label>, LabelManager<Label>>();

            // Discuss view providers
            services.AddScoped<IViewProviderManager<Doc>, ViewProviderManager<Doc>>();
            services.AddScoped<IViewProvider<Doc>, DocViewProvider>();

            // Admin view providers
            services.AddScoped<IViewProviderManager<LabelAdmin>, ViewProviderManager<LabelAdmin>>();
            services.AddScoped<IViewProvider<LabelAdmin>, AdminViewProvider>();
       
            // Register view adapters
            services.AddScoped<IViewAdapterProvider, DocListItemViewAdapter>();

            // Labels view providers
            services.AddScoped<IViewProviderManager<Label>, ViewProviderManager<Label>>();
            services.AddScoped<IViewProvider<Label>, LabelViewProvider>();

            // Label service
            services.AddScoped<ILabelService<Label>, LabelService<Label>>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "DocsLabelIndex",
                areaName: "Plato.Docs.Labels",
                template: "docs/labels/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "DocsLabelDisplay",
                areaName: "Plato.Docs.Labels",
                template: "docs/label/{opts.labelId:int}/{opts.alias?}/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Display" }
            );

        }

    }

}