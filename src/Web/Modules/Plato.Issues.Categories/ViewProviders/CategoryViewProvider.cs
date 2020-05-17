﻿using System;
using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using Plato.Issues.Categories.Models;
using Plato.Issues.Categories.ViewModels;
using Plato.Entities.ViewModels;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Issues.Models;

namespace Plato.Issues.Categories.ViewProviders
{
    public class CategoryViewProvider : ViewProviderBase<Category>
    {
        
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly IFeatureFacade _featureFacade;
        
        public CategoryViewProvider(
            ICategoryStore<Category> categoryStore,
            IFeatureFacade featureFacade)
        {
            _categoryStore = categoryStore;
            _featureFacade = featureFacade;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(Category categoryAdmin, IViewProviderContext context)
        {

            // Ensure we explicitly set the featureId
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Issues.Categories");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

         
            Category existingCategory = null;
            if (categoryAdmin?.Id > 0)
            {
                existingCategory = await _categoryStore.GetByIdAsync(categoryAdmin.Id);
            }

            // Get topic index view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Issue>)] as EntityIndexViewModel<Issue>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Issue>).ToString()} has not been registered on the HttpContext!");
            }
            
            // category filter options
            var categoryViewOpts = new CategoryIndexOptions
            {
                CategoryId = existingCategory?.Id ?? 0,
                FeatureId = feature.Id
            };
            
            var indexViewModel = new CategoryIndexViewModel()
            {
                Options = categoryViewOpts,
                EntityIndexOptions = viewModel?.Options,
                Pager = viewModel?.Pager
            };

            return Views(
                View<CategoryBase>("Home.Index.Header", model => existingCategory).Zone("header").Order(1),
                View<CategoryBase>("Home.Index.Tools", model => existingCategory).Zone("header-right").Order(1),
                View<CategoryIndexViewModel>("Home.Index.Content", model => indexViewModel).Zone("content").Order(1),
                View<CategoryIndexViewModel>("Issues.Categories.Sidebar", model => indexViewModel).Zone("content-right").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Category indexViewModel, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Category category, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Category category, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
    }
}
