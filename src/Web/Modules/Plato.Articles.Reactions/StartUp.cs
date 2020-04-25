using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Badges.Abstractions;
using PlatoCore.Models.Badges;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Tasks.Abstractions;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Services;
using Plato.Articles.Reactions.Handlers;
using Plato.Articles.Reactions.Navigation;
using Plato.Articles.Reactions.Badges;
using Plato.Articles.Reactions.Tasks;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Articles.Reactions
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

            // Register navigation provider
            services.AddScoped<INavigationProvider, ArticleMenu>();
            services.AddScoped<INavigationProvider, ArticleFooterMenu>();
            services.AddScoped<INavigationProvider, CommentMenu>();
            services.AddScoped<INavigationProvider, CommentFooterMenu>();
         
            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();
                    
            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, ReactionBadges>();

            // Background tasks
            services.AddScoped<IBackgroundTaskProvider, ReactionBadgesAwarder>();

            // Reaction providers
            services.AddScoped<IReactionsProvider<Reaction>, Reactions>();

        }

    }

}