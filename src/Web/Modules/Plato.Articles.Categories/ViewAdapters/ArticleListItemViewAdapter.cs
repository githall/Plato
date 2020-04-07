using System.Linq;
using System.Threading.Tasks;
using Plato.Categories.Stores;
using Plato.Articles.Categories.Models;
using Plato.Articles.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using System;
using System.Collections.Generic;

namespace Plato.Articles.Categories.ViewAdapters
{

    public class ArticleListItemViewAdapter : ViewAdapterProviderBase
    {

   
        private readonly ICategoryStore<Category> _channelStore;
        private readonly IFeatureFacade _featureFacade;

        private IEnumerable<Category> _categories;

        public ArticleListItemViewAdapter(
            ICategoryStore<Category> channelStore,
            IFeatureFacade featureFacade)
        {
            _channelStore = channelStore;
            _featureFacade = featureFacade;
            ViewName = "ArticleListItem";
        }

        public override async Task<IViewAdapterResult> ConfigureAsync(string viewName)
        {

            if (!viewName.Equals(ViewName, StringComparison.OrdinalIgnoreCase))
            {
                return default(IViewAdapterResult);
            }

            // Plato.Articles does not have a dependency on Plato.Articles.Categories
            // Instead we update the model for the article item view component
            // here via our view adapter to include the category information
            // This way the category data is only ever populated if the categories feature is enabled
            return await AdaptAsync(ViewName, v =>
            {
                v.AdaptModel<EntityListItemViewModel<Article>>(async model =>
                {
                    
                    if (model == null)
                    {
                        // Return an anonymous type, we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    // Get all categories for feature
                    if (_categories == null)
                    {

                        // Get feature
                        var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Articles.Categories");
                        if (feature == null)
                        {
                            // Return an anonymous type, we are adapting a view component
                            return new
                            {
                                model
                            };
                        }

                        _categories = await _channelStore.GetByFeatureIdAsync(feature.Id);

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

                    // Get our channel
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
