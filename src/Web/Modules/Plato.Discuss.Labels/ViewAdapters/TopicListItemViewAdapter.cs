using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Discuss.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Data.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.Stores;
using Plato.Discuss.Labels.Models;

namespace Plato.Discuss.Labels.ViewAdapters
{

    public class TopicListItemViewAdapter : ViewAdapterProviderBase
    {

        private readonly IEntityLabelStore<EntityLabel> _entityLabelStore;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ILabelStore<Label> _labelStore;
        private readonly IFeatureFacade _featureFacade;

        public TopicListItemViewAdapter(    
            IEntityLabelStore<EntityLabel> entityLabelStore,         
            IActionContextAccessor actionContextAccessor,
            ILabelStore<Label> labelStore,
            IFeatureFacade featureFacade)
        {
            _actionContextAccessor = actionContextAccessor;
            _entityLabelStore = entityLabelStore;
            _featureFacade = featureFacade;
            _labelStore = labelStore;
            ViewName = "TopicListItem";
        }

        private IDictionary<int, IList<Label>> _lookUp;

        public override async Task<IViewAdapterResult> ConfigureAsync(string viewName)
        {

            if (!viewName.Equals(ViewName, StringComparison.OrdinalIgnoreCase))
            {
                return default(IViewAdapterResult);
            }

            // Plato.Discuss does not have a dependency on Plato.Discuss.Labels
            // Instead we update the model for the entity list item view component
            // here via our view adapter to include the label data for the entity
            // This way the label data is only ever populated if the labels feature is enabled
            return await AdaptAsync(viewName, v =>
            {
                v.AdaptModel<EntityListItemViewModel<Topic>>(async model =>
                {

                    if (_lookUp == null)
                    {

                        // Get feature
                        var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");
                        if (feature == null)
                        {
                            // Return an anonymous type, we are adapting a view component
                            return new
                            {
                                model
                            };
                        }

                        // Get all labels for feature
                        var labels = await _labelStore.GetByFeatureIdAsync(feature.Id);
                        if (labels == null)
                        {
                            // Return an anonymous type, we are adapting a view component
                            return new
                            {
                                model
                            };
                        }

                        _lookUp = await BuildLookUpTable(labels);

                    }

                    // No need to modify if we don't have a lookup table
                    if (_lookUp == null)
                    {
                        // Return an anonymous type as we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    if (model.Entity == null)
                    {
                        // Return an anonymous type as we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    // No need to modify the model if no labels have been found
                    if (!_lookUp.ContainsKey(model.Entity.Id))
                    {
                        // Return an anonymous type as we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    // Get labels for entity
                    var entityLabels = _lookUp[model.Entity.Id];

                    // Add labels to the model from our dictionary
                    var modelLabels = new List<Label>();
                    foreach (var label in entityLabels)
                    {
                        modelLabels.Add(label);
                    }

                    model.Labels = modelLabels;

                    // Return an anonymous type as we are adapting a view component
                    return new
                    {
                        model
                    };

                });
            });

        }

        async Task<IDictionary<int, IList<Label>>> BuildLookUpTable(IEnumerable<Label> labels)
        {

            // Get index view model from context
            var viewModel = _actionContextAccessor.ActionContext.HttpContext.Items[typeof(EntityIndexViewModel<Topic>)] as EntityIndexViewModel<Topic>;
            if (viewModel == null)
            {
                return null;
            }

            // We need results
            if (viewModel.Results == null)
            {
                return null;
            }

            // Get entities for current view
            var entities = viewModel.Results;

            // Get all entity label relationships for displayed entities
            IPagedResults<EntityLabel> entityLabels = null;
            if (entities?.Data != null)
            {
                entityLabels = await _entityLabelStore.QueryAsync()
                    .Take(int.MaxValue, false)
                    .Select<EntityLabelQueryParams>(q =>
                    {
                        q.EntityId.IsIn(entities.Data.Select(e => e.Id).ToArray());
                    })
                    .ToList();
            }

            // Build a dictionary of entity and label relationships
            var output = new ConcurrentDictionary<int, IList<Label>>();
            if (entityLabels?.Data != null)
            {
                var labelList = labels.ToList();
                foreach (var entityLabel in entityLabels.Data)
                {
                    var label = labelList.FirstOrDefault(l => l.Id == entityLabel.LabelId);
                    if (label != null)
                    {
                        output.AddOrUpdate(entityLabel.EntityId, new List<Label>()
                        {
                            label
                        }, (k, v) =>
                        {
                            v.Add(label);
                            return v;
                        });
                    }
                }
            }

            return output;

        }

    }

}
