using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Messaging.Abstractions;
using Plato.Issues.Slack.Subscribers;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Issues.Slack
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