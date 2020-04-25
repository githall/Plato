using Microsoft.Extensions.DependencyInjection;
using Plato.Docs.Mentions.Notifications;
using Plato.Docs.Mentions.NotificationTypes;
using Plato.Docs.Mentions.Subscribers;
using Plato.Docs.Models;
using PlatoCore.Models.Shell;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Docs.Mentions
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
                                         
            // Register message broker subscribers
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Doc>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<DocComment>>();

            // Register notification providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Doc>, NotificationManager<Doc>>();
            services.AddScoped<INotificationManager<DocComment>, NotificationManager<DocComment>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Doc>, NewEntityMentionEmail>();
            services.AddScoped<INotificationProvider<Doc>, NewEntityMentionWeb>();
            services.AddScoped<INotificationProvider<DocComment>, NewReplyMentionWeb>();
            services.AddScoped<INotificationProvider<DocComment>, NewReplyMentionEmail>();

        }

    }

}