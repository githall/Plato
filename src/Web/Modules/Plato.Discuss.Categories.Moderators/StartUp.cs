using Microsoft.Extensions.DependencyInjection;
using Plato.Discuss.Models;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using Plato.Discuss.Categories.Moderators.Navigation;
using Plato.Discuss.Categories.Moderators.ViewProviders;
using PlatoCore.Features.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Moderation.Models;
using Plato.Discuss.Categories.Moderators.Handlers;
using Plato.Discuss.Categories.Moderators.ViewAdapters;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Discuss.Categories.Moderators
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

            // Admin view providers
            services.AddScoped<IViewProviderManager<Moderator>, ViewProviderManager<Moderator>>();
            services.AddScoped<IViewProvider<Moderator>, AdminViewProvider>();

            // Discuss view providers
            services.AddScoped<IViewProviderManager<Topic>, ViewProviderManager<Topic>>();
            services.AddScoped<IViewProvider<Topic>, TopicViewProvider>();

            // Moderation view providers
            services.AddScoped<IViewProviderManager<Moderator>, ViewProviderManager<Moderator>>();
            services.AddScoped<IViewProvider<Moderator>, ModeratorViewProvider>();

            // View Adapters
            services.AddScoped<IViewAdapterProvider, ModerationViewAdapterProvider>();

            // Register permissions providers
            services.AddScoped<IPermissionsProvider<ModeratorPermission>, ModeratorPermissions>();
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

            // Register navigation providers
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<INavigationProvider, TopicMenu>();
            services.AddScoped<INavigationProvider, TopicReplyMenu>();

        }

    }

}