using Microsoft.Extensions.DependencyInjection;
using Plato.Categories.Roles.Services;
using Plato.Discuss.Categories.Models;
using Plato.Discuss.Categories.Roles.QueryAdapters;
using Plato.Discuss.Categories.Roles.ViewProviders;
using Plato.Discuss.Models;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Stores.Abstractions.QueryAdapters;
using Plato.Discuss.Categories.Roles.Handlers;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Discuss.Categories.Roles
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
            services.AddScoped<IQueryAdapterProvider<Topic>, TopicQueryAdapter>();
            services.AddScoped<IQueryAdapterProvider<Category>, CategoryQueryAdapter>();

            // Services
            services.AddScoped<IDefaultCategoryRolesManager<Category>, DefaultCategoryRolesManager<Category>>();

        }

    }

}