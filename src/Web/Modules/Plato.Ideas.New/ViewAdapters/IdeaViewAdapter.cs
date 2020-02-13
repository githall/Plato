using System;
using System.Threading.Tasks;
using Plato.Ideas.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Localization;
using PlatoCore.Layout.TagHelperAdapters.Abstractions;
using PlatoCore.Hosting.Abstractions;
using Plato.Entities.Metrics.Repositories;
using Plato.Entities.Metrics.Extensions;
using Plato.Entities.Extensions;

namespace Plato.Ideas.New.ViewAdapters
{

    public class IdeaViewAdapter : ViewAdapterProviderBase
    {

        private readonly IAggregatedEntityMetricsRepository _agggregatedEntityMetricsRepository;     
        private readonly IContextFacade _contextFacade;

        public IHtmlLocalizer T { get; }

        public IdeaViewAdapter(
            IHtmlLocalizer<IdeaListItemViewAdapter> localizer,
            IAggregatedEntityMetricsRepository agggregatedEntityMetricsRepository,     
            IContextFacade contextFacade)
        {

            _agggregatedEntityMetricsRepository = agggregatedEntityMetricsRepository;    
            _contextFacade = contextFacade;

            T = localizer;
            ViewName = "Idea";

        }

        IDictionary<int, DateTimeOffset?> _lastVisits;

        public override async Task<IViewAdapterResult> ConfigureAsync(string viewName)
        {

            // Ensure adapter is for current view
            if (!viewName.Equals(ViewName, StringComparison.OrdinalIgnoreCase))
            {
                return default(IViewAdapterResult);
            }

            return await AdaptAsync(ViewName, v =>
            {
                v.AdaptModel<EntityViewModel<Idea, IdeaComment>>(async model =>
                {

                    if (model.Entity == null)
                    {
                        // Return an anonymous type, we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    // Build last visits from metrics
                    if (_lastVisits == null)
                    {

                        // Get authenticated user
                        var user = await _contextFacade.GetAuthenticatedUserAsync();

                        // We need to be authenticated
                        if (user == null)
                        {
                            // Return an anonymous type, we are adapting a view component
                            return new
                            {
                                model
                            };
                        }

                        _lastVisits = await _agggregatedEntityMetricsRepository.SelectMaxViewDateForEntitiesAsync(user.Id, new int[] { model.Entity.Id } );

                    }

                    // No metrics available to adapt the view
                    if (_lastVisits == null)
                    {
                        // Return an anonymous type, we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    // No metrics available to adapt the view
                    if (_lastVisits.Count == 0)
                    {
                        // Return an anonymous type, we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    DateTimeOffset? lastVisit = null;
                    if (_lastVisits.ContainsKey(model.Entity.Id))
                    {
                        lastVisit = _lastVisits[model.Entity.Id];
                    }

                    // Build tag adapters
                    var adapters = new[]
                    {
                        // Add a "New" label after the title tag 
                        new TagHelperAdapter("title", (context, output) =>
                        {
                            if (lastVisit != null)
                            {

                                // Optionally remove bold title for read entities
                                // output.Attributes.RemoveAll("class");

                                // New
                                if (model.Entity.LastReplyAfter(lastVisit))
                                {
                                    output.PostElement.SetHtmlContent(
                                        $"<span data-toggle=\"tooltip\" title=\"{T["This idea has new replies"].Value}\" class=\"badge badge-primary ml-2\">{T["New"].Value}</span>");
                                }
                                else
                                {
                                    // Modified
                                    if (model.Entity.ModifiedAfter(lastVisit))
                                    {
                                        output.PostElement.SetHtmlContent(
                                            $"<span data-toggle=\"tooltip\" title=\"{T["This idea has been updated since it was last read"].Value}\" class=\"badge badge-secondary ml-2\">{T["Updated"].Value}</span>");
                                    }
                                }
                            }                         
                        })
                    };

                    // Add tag adapters
                    model.TagHelperAdapters.Add(adapters);

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
