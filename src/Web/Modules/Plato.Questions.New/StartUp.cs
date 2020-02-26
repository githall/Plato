using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using Plato.Questions.New.ViewAdapters;

namespace Plato.Questions.New
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
            services.AddScoped<IViewAdapterProvider, QuestionListItemViewAdapter>();
            services.AddScoped<IViewAdapterProvider, QuestionViewAdapter>();
            //services.AddScoped<IViewAdapterProvider, QuestionListViewAdapter>();            
            //services.AddScoped<IViewAdapterProvider, QuestionAnswerListViewAdapter>();
            //services.AddScoped<IViewAdapterProvider, QuestionAnswerListItemViewAdapter>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}