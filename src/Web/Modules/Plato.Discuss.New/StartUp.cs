using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using Plato.Discuss.New.ViewAdapters;

namespace Plato.Discuss.New
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
            services.AddScoped<IViewAdapterProvider, TopicListItemViewAdapter>();
            //services.AddScoped<IViewAdapterProvider, TopicViewAdapter>();
            //services.AddScoped<IViewAdapterProvider, TopicListViewAdapter>();
            //services.AddScoped<IViewAdapterProvider, TopicReplyListViewAdapter>();
            //services.AddScoped<IViewAdapterProvider, TopicReplyListItemViewAdapter>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}