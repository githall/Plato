using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using Plato.Data.Tracing.ActionFilters;
using PlatoCore.Layout.ActionFilters;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Data.Tracing
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
            // Action filter
            services.AddScoped<IModularActionFilter, TracingFilter>();
        }

    }

}