using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using Plato.Issues.New.ViewAdapters;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Issues.New
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

            // Register view adapters        
            services.AddScoped<IViewAdapterProvider, IssueListItemViewAdapter>();            

            //services.AddScoped<IViewAdapterProvider, IssueViewAdapter>();
            //services.AddScoped<IViewAdapterProvider, IssueListViewAdapter>();
            //services.AddScoped<IViewAdapterProvider, IssueCommentListViewAdapter>();
            //services.AddScoped<IViewAdapterProvider, IssueCommentListItemViewAdapter>();

        }

    }

}