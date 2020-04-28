using Microsoft.Extensions.DependencyInjection;
using Plato.Discuss.Mentions.Notifications;
using Plato.Discuss.Mentions.NotificationTypes;
using Plato.Discuss.Mentions.Subscribers;
using Plato.Discuss.Models;
using PlatoCore.Models.Shell;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Discuss.Mentions
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
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Topic>>();
            services.AddScoped<IBrokerSubscriber, EntityReplySubscriber<Reply>>();

            // Register notification providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Topic>, NotificationManager<Topic>>();
            services.AddScoped<INotificationManager<Reply>, NotificationManager<Reply>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Topic>, NewEntityMentionEmail>();
            services.AddScoped<INotificationProvider<Topic>, NewEntityMentionWeb>();
            services.AddScoped<INotificationProvider<Reply>, NewReplyMentionWeb>();
            services.AddScoped<INotificationProvider<Reply>, NewReplyMentionEmail>();

        }

    }

}