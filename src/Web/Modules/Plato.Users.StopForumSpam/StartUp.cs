using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Models.Users;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.Users.StopForumSpam.Notifications;
using Plato.Users.StopForumSpam.NotificationTypes;
using Plato.Users.StopForumSpam.ViewProviders;
using Plato.Users.StopForumSpam.SpamOperators;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Users.StopForumSpam
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
            
            // Register spam operations 
            services.AddScoped<ISpamOperationProvider<SpamOperation>, SpamOperations>();
            
            // Register spam operator manager for users
            services.AddScoped<ISpamOperatorManager<User>, SpamOperatorManager<User>>();

            // Register spam operators
            services.AddScoped<ISpamOperatorProvider<User>, RegistrationOperator>();
            services.AddScoped<ISpamOperatorProvider<User>, LoginOperator>();

            // Login view provider
            services.AddScoped<IViewProviderManager<LoginPage>, ViewProviderManager<LoginPage>>();
            services.AddScoped<IViewProvider<LoginPage>, LoginViewProvider>();

            // Registration view provider
            services.AddScoped<IViewProviderManager<UserRegistration>, ViewProviderManager<UserRegistration>>();
            services.AddScoped<IViewProvider<UserRegistration>, RegisterViewProvider>();

            // Admin view provider
            services.AddScoped<IViewProviderManager<User>, ViewProviderManager<User>>();
            services.AddScoped<IViewProvider<User>, AdminViewProvider>();

            // Notification types
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification manager
            services.AddScoped<INotificationManager<User>, NotificationManager<User>>();

            // Notification providers
            services.AddScoped<INotificationProvider<User>, NewSpamWeb>();
            services.AddScoped<INotificationProvider<User>, NewSpamEmail>();

        }

    }

}