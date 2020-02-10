using System;
using System.Linq;
using System.Threading.Tasks;
using Plato.Categories.Stores;
using Plato.Discuss.Models;
using Plato.Entities.ViewModels;
using Plato.Discuss.Categories.Models;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout.ViewAdapters;
using System.Collections.Generic;

namespace Plato.Discuss.Categories.ViewAdapters
{

    public class TopicListItemViewAdapter : ViewAdapterProviderBase
    {
             
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly IFeatureFacade _featureFacade;

        public TopicListItemViewAdapter(
            ICategoryStore<Category> categoryStore,
            IFeatureFacade featureFacade)
        {
            _categoryStore = categoryStore;
            _featureFacade = featureFacade;
            ViewName = "TopicListItem";
        }

        IEnumerable<Category> _categories;

        public override async Task<IViewAdapterResult> ConfigureAsync(string viewName)
        {

            if (!viewName.Equals(ViewName, StringComparison.OrdinalIgnoreCase))
            {
                return default(IViewAdapterResult);
            }
     
            // Plato.Discuss does not have a dependency on Plato.Discuss.Categories
            // Instead we update the model for the topic item view component
            // here via our view adapter to include the channel information
            // This way the channel data is only ever populated if the channels feature is enabled
            return await AdaptAsync(ViewName, v =>
            {
                v.AdaptModel<EntityListItemViewModel<Topic>>(async model =>
                {

                    if (_categories == null)
                    {
                        // Get feature
                        var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Categories");
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
