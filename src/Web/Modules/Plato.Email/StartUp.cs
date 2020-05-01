﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Email.Configuration;
using Plato.Email.Handlers;
using Plato.Email.Navigation;
using Plato.Email.Repositories;
using Plato.Email.Stores;
using Plato.Email.Subscribers;
using Plato.Email.Tasks;
using Plato.Email.ViewProviders;
using PlatoCore.Abstractions.SetUp;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Tasks.Abstractions;
using PlatoCore.Stores.Abstractions.QueryAdapters;
using PlatoCore.Stores;
using PlatoCore.Stores.Abstractions.FederatedQueries;
using PlatoCore.Data.Migrations.Abstractions;
using PlatoCore.Hosting.Abstractions;
using Plato.Email.Services;

namespace Plato.Email
{
    public class Startup : StartupBase
    {     

        public override void ConfigureServices(IServiceCollection services)
        {

            // Feature installation event handler            
            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // Navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // Repositories
            services.AddScoped<IEmailRepository<EmailMessage>, EmailRepository>();
            services.AddScoped<IEmailAttachmentRepository<EmailAttachment>, EmailAttachmentRepository>();

            // Stores
            services.AddScoped<IEmailAttachmentStore<EmailAttachment>, EmailAttachmentStore>();
            services.AddScoped<IEmailSettingsStore<EmailSettings>, EmailSettingsStore>();
            services.AddScoped<IEmailStore<EmailMessage>, EmailStore>();
            services.AddScoped<IEmailSettingsManager, EmailSettingsManager>();

            // View providers
            services.AddScoped<IViewProviderManager<EmailSettings>, ViewProviderManager<EmailSettings>>();
            services.AddScoped<IViewProvider<EmailSettings>, AdminViewProvider>();

            // Configuration
            services.AddTransient<IConfigureOptions<SmtpSettings>, SmtpSettingsConfiguration>();

            // Services
            services.AddScoped<ISmtpService, SmtpService>();    
        
            // Email subscribers
            services.AddScoped<IBrokerSubscriber, EmailSubscriber>();

            // Email manager
            services.AddSingleton<IEmailManager, EmailManager>();

            // Query adapters
            services.AddScoped<IQueryAdapterManager<EmailAttachment>, QueryAdapterManager<EmailAttachment>>();

            // Federated queries
            services.AddScoped<IFederatedQueryManager<EmailAttachment>, FederatedQueryManager<EmailAttachment>>();

            // Background Tasks
            services.AddScoped<IBackgroundTaskProvider, EmailSender>();

            // Permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

            // Migrations
            services.AddSingleton<IMigrationProvider, Migrations>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "PlatoEmailAdmin",
                areaName: "Plato.Email",
                template: "admin/settings/email",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }

    }

}