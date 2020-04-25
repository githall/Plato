using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Models.Shell;
using PlatoCore.Models.Users;
using PlatoCore.Security.Abstractions;
using Plato.Users.Social.ViewProviders;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Users.Social
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
            services.AddScoped<IViewProviderManager<User>, ViewProviderManager<User>>();
            services.AddScoped<IViewProvider<User>, UserViewProvider>();

            // Module permissions
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

    }

}