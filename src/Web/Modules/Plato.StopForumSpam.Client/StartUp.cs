using Microsoft.Extensions.DependencyInjection;
using Plato.StopForumSpam.Client.Services;
using PlatoCore.Hosting.Abstractions;

namespace Plato.StopForumSpam.Client
{
    public class Startup : StartupBase
    {

        public override void ConfigureServices(IServiceCollection services)
        {
            // Register StopForumSpam services
            services.AddScoped<ISpamClient, SpamClient>();
            services.AddScoped<ISpamProxy, SpamProxy>();
        }

    }

}