using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using Plato.Discuss.TopParticipants.ViewProviders;
using Plato.Discuss.Models;

namespace Plato.Discuss.TopParticipants
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
            
            // View providers
            services.AddScoped<IViewProviderManager<Topic>, ViewProviderManager<Topic>>();
            services.AddScoped<IViewProvider<Topic>, TopicViewProvider>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}