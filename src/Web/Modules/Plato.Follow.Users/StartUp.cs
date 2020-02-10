using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Follows.Services;
using Plato.Follow.Users.Subscribers;
using Plato.Follow.Users.ViewProviders;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Models.Users;

namespace Plato.Follow.Users
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

            // User profile View providers
            services.AddScoped<IViewProviderManager<ProfilePage>, ViewProviderManager<ProfilePage>>();
            services.AddScoped<IViewProvider<ProfilePage>, ProfileViewProvider>();

            // Follow types
            services.AddScoped<IFollowTypeProvider, FollowTypes>();

            // Follow subscribers
            services.AddScoped<IBrokerSubscriber, FollowSubscriber>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}