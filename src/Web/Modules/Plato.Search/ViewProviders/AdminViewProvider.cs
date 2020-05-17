﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Shell;
using Plato.Search.Models;
using Plato.Search.Stores;
using Plato.Search.ViewModels;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Search.ViewProviders
{
    public class AdminViewProvider : ViewProviderBase<SearchSettings>
    {

        private readonly ISearchSettingsStore<SearchSettings> _searchSettingsStore;
        private readonly IFullTextCatalogStore _fullTextCatalogStore;
        private readonly IFullTextIndexStore _fullTextIndexStore;        
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;

        public AdminViewProvider(
            ISearchSettingsStore<SearchSettings> searchSettingsStore,
            IFullTextCatalogStore fullTextCatalogStore,
            IFullTextIndexStore fullTextIndexStore,            
            IShellSettings shellSettings,
            IPlatoHost platoHost)
        {
            _searchSettingsStore = searchSettingsStore;
            _fullTextCatalogStore = fullTextCatalogStore;
            _fullTextIndexStore = fullTextIndexStore;
            _shellSettings = shellSettings;
            _platoHost = platoHost;
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(SearchSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(SearchSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(SearchSettings settings, IViewProviderContext context)
        {

            var viewModel = await GetModel();
            return Views(
                View<SearchSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<SearchSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("header-right").Order(1),
                View<SearchSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(SearchSettings settings,
            IViewProviderContext context)
        {
            var model = new SearchSettingsViewModel();

            // Validate model
            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(settings, context);
            }

            // Update settings
            if (context.Updater.ModelState.IsValid)
            {
                var result = await _searchSettingsStore.SaveAsync(new SearchSettings()
                {
                    SearchType = model.SearchType
                });
                if (result != null)
                {
                    // Recycle shell context to ensure changes take effect
                    _platoHost.RecycleShell(_shellSettings);
                }
            }

            return await BuildEditAsync(settings, context);

        }

        async Task<SearchSettingsViewModel> GetModel()
        {

            var model = new SearchSettingsViewModel
            {
                AvailableSearchTypes = GetAvailableSearchTypes(),
                Catalogs = await _fullTextCatalogStore.SelectCatalogsAsync(),
                Indexes = await _fullTextIndexStore.SelectIndexesAsync()
            };

            var settings = await _searchSettingsStore.GetAsync();
            if (settings != null)
            {
                model.SearchType = settings.SearchType;

            }

            return model;

        }
        
        IEnumerable<SelectListItem> GetAvailableSearchTypes()
        {

            var output = new List<SelectListItem>();
            foreach (var searchType in SearchDefaults.AvailableSearchTypes)
            {
                output.Add(new SelectListItem
                {
                    Text = searchType.Name,
                    Value = System.Convert.ToString((int)searchType.Type)
                });
            }

            return output;

        }

    }

}
