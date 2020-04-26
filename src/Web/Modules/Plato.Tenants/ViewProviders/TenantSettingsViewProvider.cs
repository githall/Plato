using System;
using System.Threading.Tasks;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Tenants.ViewModels;
using Plato.Tenants.Models;
using Microsoft.Extensions.Logging;
using Plato.Tenants.Stores;

namespace Plato.Tenants.ViewProviders
{

    public class TenantSettingsViewProvider : ViewProviderBase<TenantSettings>
    {

        private readonly ITenantSettingsStore<TenantSettings> _tenantSettingsStore;
        private readonly ILogger<AdminViewProvider> _logger;

        public TenantSettingsViewProvider(
            ITenantSettingsStore<TenantSettings> tenantSettingsStore,
            ILogger<AdminViewProvider> logger)
        {
            _tenantSettingsStore = tenantSettingsStore;
            _logger = logger;
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(TenantSettings settings, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(TenantSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(TenantSettings settings, IViewProviderContext updater)
        {

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            // Build view model
            var viewModel = await GetModel();
            return Views(
                View<EditTenantSettingsViewModel>("Admin.EditSettings.Header", model => viewModel).Zone("header"),
                View<EditTenantSettingsViewModel>("Admin.EditSettings.Content", model => viewModel).Zone("content"),
                View<EditTenantSettingsViewModel>("Admin.EditSettings.Footer", model => viewModel).Zone("footer")
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(TenantSettings settings, IViewProviderContext context)
        {
            return await BuildEditAsync(settings, context);
        }

        // -----------------

        private async Task<EditTenantSettingsViewModel> GetModel()
        {
            var settings = await _tenantSettingsStore.GetAsync();
            if (settings != null)
            {
                return new EditTenantSettingsViewModel()
                {
                    ConnectionString = settings.ConnectionString
                };
            }

            return new EditTenantSettingsViewModel();

        }

    }

}

