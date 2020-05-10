using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using Plato.Issues.Categories.Models;
using Plato.Issues.Categories.ViewModels;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;

namespace Plato.Issues.Categories.ViewProviders
{

    public class AdminViewProvider : ViewProviderBase<CategoryAdmin>
    {        

        private readonly ICategoryManager<Category> _categoryManager;
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly IFeatureFacade _featureFacade;

        public IStringLocalizer S { get; }

        public AdminViewProvider(
            IStringLocalizer stringLocalizer,
            ICategoryManager<Category> categoryManager,
            ICategoryStore<Category> categoryStore,
            IFeatureFacade featureFacade)
        {

            _categoryManager = categoryManager;
            _featureFacade = featureFacade;
            _categoryStore = categoryStore;            

            S = stringLocalizer;

        }

        public override async Task<IViewProviderResult> BuildIndexAsync(CategoryAdmin categoryBase, IViewProviderContext updater)
        {

            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Issues.Categories");
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the Id 'Plato.Issues.Categories'");
            }

            var viewModel = new CategoryIndexViewModel()
            {
                Options = new CategoryIndexOptions()
                {
                    FeatureId = feature.Id,
                    CategoryId = categoryBase?.Id ?? 0
                }
            };

            return Views(
                View<CategoryBase>("Admin.Index.Header", model => categoryBase).Zone("header").Order(1),
                View<CategoryIndexViewModel>("Admin.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<CategoryIndexViewModel>("Admin.Index.Content", model => viewModel).Zone("content").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(CategoryAdmin viewModel, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override async Task<IViewProviderResult> BuildEditAsync(CategoryAdmin categoryBase, IViewProviderContext updater)
        {

            var defaultIcons = new DefaultIcons();

            EditCategoryViewModel editCategoryViewModel = null;
            if (categoryBase.Id == 0)
            {
                editCategoryViewModel = new EditCategoryViewModel()
                {
                    IconPrefix = defaultIcons.Prefix,
                    ChannelIcons = defaultIcons,
                    IsNewCategory = true,
                    ParentId = categoryBase.ParentId,
                    AvailableChannels = await GetAvailableCategoriesAsync()
                };
            }
            else
            {
                editCategoryViewModel = new EditCategoryViewModel()
                {
                    Id = categoryBase.Id,
                    ParentId = categoryBase.ParentId,
                    Name = categoryBase.Name,
                    Description = categoryBase.Description,
                    ForeColor = categoryBase.ForeColor,
                    BackColor = categoryBase.BackColor,
                    IconCss = categoryBase.IconCss,
                    IconPrefix = defaultIcons.Prefix,
                    ChannelIcons = defaultIcons,
                    AvailableChannels = await GetAvailableCategoriesAsync()
                };
            }

            return Views(
                View<EditCategoryViewModel>("Admin.Edit.Header", model => editCategoryViewModel).Zone("header").Order(1),
                View<EditCategoryViewModel>("Admin.Edit.Content", model => editCategoryViewModel).Zone("content").Order(1),                
                View<EditCategoryViewModel>("Admin.Edit.Footer", model => editCategoryViewModel).Zone("actions").Order(1),
                View<EditCategoryViewModel>("Admin.Edit.Actions", model => editCategoryViewModel).Zone("actions-right").Order(1)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(CategoryAdmin categoryBase, IViewProviderContext context)
        {

            var model = new EditCategoryViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(categoryBase, context);
            }

            model.Name = model.Name?.Trim();
            model.Description = model.Description?.Trim();
  
            if (context.Updater.ModelState.IsValid)
            {

                var iconCss = model.IconCss;
                if (!string.IsNullOrEmpty(iconCss))
                {
                    iconCss = model.IconPrefix + iconCss;
                }

                var result = await _categoryManager.UpdateAsync(new Category()
                {
                    Id = categoryBase.Id,
                    FeatureId = categoryBase.FeatureId,
                    ParentId = model.ParentId,
                    Name = model.Name,
                    Description = model.Description,
                    ForeColor = model.ForeColor,
                    BackColor = model.BackColor,
                    IconCss = iconCss,
                    SortOrder = categoryBase.SortOrder
                });

                foreach (var error in result.Errors)
                {
                    context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return await BuildEditAsync(categoryBase, context);
            
        }

        // ------------

        private async Task<IEnumerable<SelectListItem>> GetAvailableCategoriesAsync()
        {

            var output = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = S["root category"],
                    Value = "0"
                }
            };

            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Issues.Categories");
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the Id 'Plato.Issues.Categories'");
            }

            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);
            if (categories != null)
            {
                var items = RecurseCategories(categories);
                foreach (var item in items)
                {
                    output.Add(item);
                }
            }
          
            return output;

        }

        private IList<SelectListItem> RecurseCategories(
            IEnumerable<ICategory> input,
            IList<SelectListItem> output = null,
            int id = 0)
        {

            if (output == null)
            {
                output = new List<SelectListItem>();
            }
            
            var categories = input.ToList();
            foreach (var category in categories)
            {
                if (category.ParentId == id)
                {
                    var indent = "-".Repeat(category.Depth);
                    if (!string.IsNullOrEmpty(indent))
                    {
                        indent += " ";
                    }
                    output.Add(new SelectListItem
                    {
                        Text = indent + category.Name,
                        Value = category.Id.ToString()
                    });
                    RecurseCategories(categories, output, category.Id);
                }
            }

            return output;

        }

    }

}
