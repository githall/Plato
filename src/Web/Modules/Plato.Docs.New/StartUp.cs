﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using Plato.Docs.New.ViewAdapters;

namespace Plato.Docs.New
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
            services.AddScoped<IViewAdapterProvider, DocListItemViewAdapter>();
            //services.AddScoped<IViewAdapterProvider, DocViewAdapter>();
            //services.AddScoped<IViewAdapterProvider, DocListViewAdapter>();            
            //services.AddScoped<IViewAdapterProvider, DocCommentListViewAdapter>();
            //services.AddScoped<IViewAdapterProvider, DocCommentListItemViewAdapter>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}