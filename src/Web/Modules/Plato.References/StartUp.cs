using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Assets.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Messaging.Abstractions;
using Plato.References.Assets;
using Plato.References.Services;
using Plato.References.Subscribers;
using PlatoCore.Hosting.Abstractions;

namespace Plato.References
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

            // Tokenizers
            services.AddScoped<IHashTokenizer, HashTokenizer>();
            services.AddScoped<ILinkTokenizer, LinkTokenizer>();

            // Parser
            services.AddScoped<IReferencesParser, ReferencesParser>();

            // Register broker subscribers
            services.AddScoped<IBrokerSubscriber, ParseEntityHtmlSubscriber>();

            // Register assets
            services.AddScoped<IAssetProvider, AssetProvider>();
            
        }

    }

}