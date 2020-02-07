using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Categories.Stores;
using Plato.Issues.Categories.Models;
using Plato.Issues.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout.ViewAdapters;

namespace Plato.Issues.Categories.ViewAdapters
{

    public class IssueListItemViewAdapter : BaseAdapterProvider
    {
           
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly IFeatureFacade _featureFacade;

        public IssueListItemViewAdapter(
            ICategoryStore<Category> categoryStore,
            IFeatureFacade featureFacade)
        {
            _categoryStore = categoryStore;
            _featureFacade = featureFacade;
            ViewName = "IssueListItem"; 
        }

        IEnumerable<Category> _categories;

        public override async Task<IViewAdapterResult> ConfigureAsync(string viewName)
        {

            if (!viewName.Equals(ViewName, StringComparison.OrdinalIgnoreCase))
            {
                return default(IViewAdapterResult);
            }

            // Plato.Issues does not have a dependency on Plato.Issues.Categories
            // Instead we update the model for the topic item view component
            // here via our view adapter to include the category information
            // This way the category data is only ever populated if the categories feature is enabled
            return await Adapt(ViewName, v =>
            {
                v.AdaptModel<EntityListItemViewModel<Issue>>(async model =>
                {

                    if (_categories == null)
                    {
                        // Get feature
                        var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Issues.Categories");
                        if (feature == null)
                        {
                            // Return an anonymous type, we are adapting a view component
                            return new
                            {
                                model
                            };
                        }

                        // Get all categories for feature
                        _categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);

                    }

                    if (_categories == null)
                    {
                        // Return an anonymous type, we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    if (model.Entity == null)
                    {
                        // Return an anonymous type, we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    // Ensure we have a category
                    if (model.Entity.CategoryId <= 0)
                    {
                        // Return an anonymous type, we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    // Get our category
                    var category = _categories.FirstOrDefault(c => c.Id == model.Entity.CategoryId);
                    if (category != null)
                    {
                        model.Category = category;
                    }
                    
                    // Return an anonymous type, we are adapting a view component
                    return new
                    {
                        model
                    };

                });
            });

        }

    }


}
