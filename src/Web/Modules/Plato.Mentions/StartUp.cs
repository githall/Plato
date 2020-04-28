using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Assets.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Messaging.Abstractions;
using Plato.Mentions.Assets;
using Plato.Mentions.Handlers;
using Plato.Mentions.Models;
using Plato.Mentions.Repositories;
using Plato.Mentions.Services;
using Plato.Mentions.Stores;
using Plato.Mentions.Subscribers;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Mentions
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
            services.AddScoped<IEntityMentionsRepository<EntityMention>, EntityMentionsRepository>();

            // Stores
            services.AddScoped<IEntityMentionsStore<EntityMention>, EntityMentionsStore>();

            // Parsers
            //services.AddScoped<IMentionsParser, MentionsParser>();
            services.AddScoped<IMentionsParser, MentionsParser>();
            services.AddScoped<IMentionsTokenizer, MentionsTokenizer>();

            // Register broker subscribers
            services.AddScoped<IBrokerSubscriber, ParseEntityHtmlSubscriber>();
            
            // Managers
            services.AddScoped<IEntityMentionsManager<EntityMention>, EntityMentionsManager>();

            // Register assets
            services.AddScoped<IAssetProvider, AssetProvider>();

        }

    }

}