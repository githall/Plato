using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Security;
using PlatoCore.Security.Abstractions;
using Plato.Moderation.Models;
using Plato.Moderation.Handlers;
using Plato.Moderation.Repositories;
using Plato.Moderation.Services;
using Plato.Moderation.Stores;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Moderation
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

            // Repositories
            services.AddScoped<IModeratorRepository<Moderator>, ModeratorRepository>();

            // Stores
            services.AddScoped<IModeratorStore<Moderator>, ModeratorStore>();
            
            // Register moderator permissions manager
            services.AddScoped<IPermissionsManager<ModeratorPermission>, PermissionsManager<ModeratorPermission>>();
          
            // Register additional authorization handler for implied permissions
            services.AddScoped<IAuthorizationHandler, ModeratorPermissionsHandler>();
            
        }

    }

}