using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Assets.Abstractions;
using Plato.Docs.Anchorific.Assets;
using Plato.Internal.Layout.ViewProviders;
using Plato.Docs.Models;
using Plato.Docs.Anchorific.ViewProviders;

namespace Plato.Docs.Anchorific
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

            // Client assets
            services.AddScoped<IAssetProvider, AssetProvider>();

            // View providers
            services.AddScoped<IViewProviderManager<Doc>, ViewProviderManager<Doc>>();
            services.AddScoped<IViewProvider<Doc>, DocViewProvider>();
            //services.AddScoped<IViewProviderManager<DocComment>, ViewProviderManager<DocComment>>();
            //services.AddScoped<IViewProvider<DocComment>, CommentViewProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}