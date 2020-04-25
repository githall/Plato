using Microsoft.Extensions.DependencyInjection;
using Plato.Discuss.Slack.Subscribers;
using PlatoCore.Models.Shell;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Discuss.Slack
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

            // Broker subscriptions
            services.AddScoped<IBrokerSubscriber, EntitySubscriber>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber>();

        }

    }

}