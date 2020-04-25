using Microsoft.Extensions.DependencyInjection;
using Plato.Questions.Reactions.Handlers;
using Plato.Questions.Reactions.Navigation;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using Plato.Questions.Reactions.Badges;
using Plato.Questions.Reactions.Tasks;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Services;
using PlatoCore.Badges.Abstractions;
using PlatoCore.Models.Badges;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Tasks.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Questions.Reactions
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
            services.AddScoped<INavigationProvider, QuestionMenu>();
            services.AddScoped<INavigationProvider, QuestionFooterMenu>();
            services.AddScoped<INavigationProvider, AnswerMenu>();
            services.AddScoped<INavigationProvider, AnswerFooterMenu>();
         
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