using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using Plato.Ideas.New.ViewAdapters;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Ideas.New
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
            services.AddScoped<IViewAdapterProvider, IdeaListItemViewAdapter>();

            //services.AddScoped<IViewAdapterProvider, IdeaViewAdapter>();
            //services.AddScoped<IViewAdapterProvider, IdeaListViewAdapter>();            
            //services.AddScoped<IViewAdapterProvider, IdeaCommentListViewAdapter>();
            //services.AddScoped<IViewAdapterProvider, IdeaCommentListItemViewAdapter>();

        }

    }

}