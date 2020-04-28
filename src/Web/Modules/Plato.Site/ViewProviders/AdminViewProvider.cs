using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Site.Models;
using Plato.Site.Stores;
using Plato.Site.ViewModels;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;

namespace Plato.Site.ViewProviders
{
    public class AdminViewProvider : ViewProviderBase<PlatoSiteSettings>
    {

        private readonly IPlatoSiteSettingsStore<PlatoSiteSettings> _demoSettingsStore;        
        private readonly ILogger<AdminViewProvider> _logger;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;

        public AdminViewProvider(
            IPlatoSiteSettingsStore<PlatoSiteSettings> demoSettingsStore,            
            ILogger<AdminViewProvider> logger,
            IShellSettings shellSettings,
            IPlatoHost platoHost)
        {            
            _demoSettingsStore = demoSettingsStore;
            _shellSettings = shellSettings;
            _platoHost = platoHost;
            _logger = logger;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(PlatoSiteSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(PlatoSiteSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(PlatoSiteSettings settings, IViewProviderContext context)
        {
            var viewModel = await GetModel();
            return Views(
                View<PlatoSiteSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<PlatoSiteSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("tools").Order(1),
                View<PlatoSiteSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)
            );
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(PlatoSiteSettings settings, IViewProviderContext context)
        {
            
            var model = new PlatoSiteSettingsViewModel();

            // Validate model
            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(settings, context);
            }
            
            // Update settings
            if (context.Updater.ModelState.IsValid)
            {

                // Create the model
                settings = new PlatoSiteSettings()
                {
                    DemoUrl = model.DemoUrl,
                    PlatoDesktopUrl = model.PlatoDesktopUrl
                };

                // Persist the settings
                var result = await _demoSettingsStore.SaveAsync(settings);
                if (result != null)
                {
                    // Recycle shell context to ensure changes take effect
                    _platoHost.RecycleShell(_shellSettings);
                }
              
            }

            return await BuildEditAsync(settings, context);

        }
        
        async Task<PlatoSiteSettingsViewModel> GetModel()
        {

            var settings = await _demoSettingsStore.GetAsync();
            if (settings != null)
            {
                return new PlatoSiteSettingsViewModel()
                {
                    DemoUrl = settings.DemoUrl,
                    PlatoDesktopUrl = settings.PlatoDesktopUrl
                };
            }

            // return default settings
            return new PlatoSiteSettingsViewModel();

        }

    }

}
