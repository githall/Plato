using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using Plato.Issues.New.ViewAdapters;

namespace Plato.Issues.New
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

            // Register view adapters        
            services.AddScoped<IViewAdapterProvider, IssueListItemViewAdapter>();
            services.AddScoped<IViewAdapterProvider, IssueListViewAdapter>();
            services.AddScoped<IViewAdapterProvider, IssueViewAdapter>();
            services.AddScoped<IViewAdapterProvider, IssueCommentListViewAdapter>();
            services.AddScoped<IViewAdapterProvider, IssueCommentListItemViewAdapter>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}