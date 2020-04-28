using Microsoft.Extensions.DependencyInjection;
using Plato.Categories.Roles.Services;
using Plato.Docs.Categories.Models;
using Plato.Docs.Categories.Roles.QueryAdapters;
using Plato.Docs.Categories.Roles.ViewProviders;
using Plato.Docs.Models;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Stores.Abstractions.QueryAdapters;
using Plato.Docs.Categories.Roles.Handlers;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Docs.Categories.Roles
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

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Category role view providers
            services.AddScoped<IViewProviderManager<CategoryAdmin>, ViewProviderManager<CategoryAdmin>>();
            services.AddScoped<IViewProvider<CategoryAdmin>, CategoryRolesViewProvider>();

            // Query adapters to limit access by role
            services.AddScoped<IQueryAdapterProvider<Doc>, DocQueryAdapter>();
            services.AddScoped<IQueryAdapterProvider<SimpleDoc>, SimpleDocQueryAdapter>();
            services.AddScoped<IQueryAdapterProvider<Category>, CategoryQueryAdapter>();            

            // Services
            services.AddScoped<IDefaultCategoryRolesManager<Category>, DefaultCategoryRolesManager<Category>>();

        }

    }

}