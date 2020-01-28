using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Notifications;
using PlatoCore.Notifications.Abstractions;
using Plato.Questions.StopForumSpam.Notifications;
using Plato.Questions.StopForumSpam.NotificationTypes;
using Plato.Questions.StopForumSpam.SpamOperators;
using Plato.Questions.StopForumSpam.ViewProviders;
using Plato.Questions.Models;
using PlatoCore.Features.Abstractions;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.Questions.StopForumSpam.Handlers;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Questions.StopForumSpam.Navigation;

namespace Plato.Questions.StopForumSpam
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

            // Navigation providers
            services.AddScoped<INavigationProvider, IssueMenu>();
            services.AddScoped<INavigationProvider, IssueCommentMenu>();

            // Register spam operations provider
            services.AddScoped<ISpamOperationProvider<SpamOperation>, SpamOperations>();

            // Register spam operator manager for topics
            services.AddScoped<ISpamOperatorManager<Question>, SpamOperatorManager<Question>>();
            services.AddScoped<ISpamOperatorManager<Answer>, SpamOperatorManager<Answer>>();

            // Register spam operators
            services.AddScoped<ISpamOperatorProvider<Question>, QuestionOperator>();
            services.AddScoped<ISpamOperatorProvider<Answer>, AnswerOperator>();

            // Register view providers
            services.AddScoped<IViewProviderManager<Question>, ViewProviderManager<Question>>();
            services.AddScoped<IViewProvider<Question>, QuestionViewProvider>();
            services.AddScoped<IViewProviderManager<Answer>, ViewProviderManager<Answer>>();
            services.AddScoped<IViewProvider<Answer>, AnswerViewProvider>();
            
            // Notification types
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification manager
            services.AddScoped<INotificationManager<Question>, NotificationManager<Question>>();
            services.AddScoped<INotificationManager<Answer>, NotificationManager<Answer>>();

            // Notification providers
            services.AddScoped<INotificationProvider<Question>, QuestionSpamWeb>();
            services.AddScoped<INotificationProvider<Question>, QuestionSpamEmail>();
            services.AddScoped<INotificationProvider<Answer>, AnswerSpamWeb>();
            services.AddScoped<INotificationProvider<Answer>, AnswerSpamEmail>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Index
            routes.MapAreaRoute(
                name: "QuestionsSpamIndex",
                areaName: "Plato.Questions.StopForumSpam",
                template: "questions/q/spam/details/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            // AddSpammer
            routes.MapAreaRoute(
                name: "QuestionsSpamSubmit",
                areaName: "Plato.Questions.StopForumSpam",
                template: "questions/q/spam/add/{opts.id:int}/{opts.alias}/{opts.replyId:int?}",
                defaults: new { controller = "Home", action = "AddSpammer" }
            );
            
        }

    }

}