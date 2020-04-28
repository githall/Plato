using Microsoft.Extensions.DependencyInjection;
using Plato.Categories.Models;
using Plato.Categories.Roles.QueryAdapters;
using PlatoCore.Stores.Abstractions.QueryAdapters;
using Plato.Categories.Roles.Services;
using PlatoCore.Features.Abstractions;
using Plato.Categories.Roles.Handlers;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Categories.Roles
{

    public class Startup : StartupBase
    {  

        public override void ConfigureServices(IServiceCollection services)
        {

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Query adapters 
            services.AddScoped<IQueryAdapterProvider<CategoryBase>, CategoryQueryAdapter>();

            // Services
            services.AddScoped<IDefaultCategoryRolesManager<CategoryBase>, DefaultCategoryRolesManager<CategoryBase>>();

        }

    }

}