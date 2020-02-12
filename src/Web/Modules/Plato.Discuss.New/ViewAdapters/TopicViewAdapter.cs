using System;
using System.Linq;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Localization;
using PlatoCore.Layout.TagHelperAdapters.Abstractions;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Hosting.Abstractions;
using Plato.Entities.Metrics.Repositories;
using Plato.Entities.Metrics.Extensions;
using Plato.Entities.Extensions;

namespace Plato.Discuss.New.ViewAdapters
{

    public class TopicViewAdapter : ViewAdapterProviderBase
    {

        private readonly IAggregatedEntityMetricsRepository _agggregatedEntityMetricsRepository;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IContextFacade _contextFacade;


        public IHtmlLocalizer T { get; }

        public TopicViewAdapter(
            IHtmlLocalizer<TopicListItemViewAdapter> localizer,
            IAggregatedEntityMetricsRepository agggregatedEntityMetricsRepository,
            IActionContextAccessor actionContextAccessor,
            IContextFacade contextFacade)
        {

            _agggregatedEntityMetricsRepository = agggregatedEntityMetricsRepository;
            _actionContextAccessor = actionContextAccessor;
            _contextFacade = contextFacade;

            T = localizer;
            ViewName = "Topic";

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
                v.AdaptModel<EntityViewModel<Topic, Reply>>(async model =>
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
                                // New
                                if (model.Entity.CreatedAfter(lastVisit) || model.Entity.LastReplyAfter(lastVisit))
                                {
                                    output.PostElement.SetHtmlContent(
                                        $"<span class=\"badge badge-primary ml-2\">{T["New"].Value}</span>");
                                }
                                else
                                {
                                    // Modified
                                    if (model.Entity.ModifiedAfter(lastVisit))
                                    {
                                        output.PostElement.SetHtmlContent(
                                            $"<span class=\"badge badge-secondary ml-2\">{T["Updated"].Value}</span>");
                                    }
                                }
                            }
                            else
                            {
                                output.PostElement.SetHtmlContent(
                                    $"<span class=\"badge badge-primary ml-2\">{T["New"].Value}</span>");
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
