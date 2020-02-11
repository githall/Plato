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

namespace Plato.Discuss.New.ViewAdapters
{

    public class TopicListItemViewAdapter : ViewAdapterProviderBase
    {

        private readonly IAggregatedEntityMetricsRepository _agggregatedEntityMetricsRepository;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IContextFacade _contextFacade;      

        public IHtmlLocalizer T { get; }

        public TopicListItemViewAdapter(
            IHtmlLocalizer<TopicListItemViewAdapter> localizer,
            IAggregatedEntityMetricsRepository agggregatedEntityMetricsRepository,
            IActionContextAccessor actionContextAccessor,         
            IContextFacade contextFacade)
        {

            _agggregatedEntityMetricsRepository = agggregatedEntityMetricsRepository;
            _actionContextAccessor = actionContextAccessor;        
            _contextFacade = contextFacade; 

            T = localizer;
            ViewName = "TopicListItem";

        }

        IDictionary<int, DateTimeOffset?> _lastVisits;

        public override async Task<IViewAdapterResult> ConfigureAsync(string viewName)
        {

            // Ensure adapter is for current view
            if (!viewName.Equals(ViewName, StringComparison.OrdinalIgnoreCase))
            {
                return default(IViewAdapterResult);
            }

            // Adapt the view
            return await AdaptAsync(ViewName, v =>
            {
                v.AdaptModel<EntityListItemViewModel<Topic>>(async model =>
                {

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

                        // Get index view model from context
                        var viewModel = _actionContextAccessor.ActionContext.HttpContext.Items[typeof(EntityIndexViewModel<Topic>)] as EntityIndexViewModel<Topic>;
                        if (viewModel == null)
                        {
                            // Return an anonymous type, we are adapting a view component
                            return new
                            {
                                model
                            };
                        }

                        if (viewModel.Results == null)
                        {
                            // Return an anonymous type, we are adapting a view component
                            return new
                            {
                                model
                            };
                        }

                        if (viewModel.Results.Data != null)
                        {
                            _lastVisits = await _agggregatedEntityMetricsRepository.SelectMaxViewDateForEntitiesAsync(user.Id, viewModel.Results.Data.Select(e => e.Id).ToArray());
                        }
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

                    if (model.Entity == null)
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

                    // Ensure tag alterations
                    if (model.TagHelperAdapters == null)
                    {
                        model.TagHelperAdapters = new TagHelperAdapterCollection();
                    }

                    // Build tag adapters
                    var adapters = new[]
                    {
                        // Add a "New" label after the title tag 
                        new TagHelperAdapter("title", (context, output) =>
                        {
                            if (lastVisit != null)
                            {

                                var suppressAlterations = false;

                                // Last reply
                                if (model.Entity.LastReplyDate.HasValue)
                                {
                                    if (model.Entity.LastReplyDate > lastVisit)
                                    {
                                        output.PostElement.SetHtmlContent(
                                            $"<span class=\"badge badge-primary ml-2\">{T["New"].Value}</span>");
                                        suppressAlterations = true;
                                    }
                                }
                                
                                // Modified
                                if (model.Entity.ModifiedDate.HasValue && !suppressAlterations)
                                {
                                    if (model.Entity.ModifiedDate > lastVisit)
                                    {
                                        output.PostElement.SetHtmlContent(
                                            $"<span class=\"badge badge-secondary ml-2\">{T["Updated"].Value}</span>");
                                        suppressAlterations = true;
                                    }
                                }

                                // Created
                                if (model.Entity.CreatedDate.HasValue && !suppressAlterations)
                                {
                                    if (model.Entity.CreatedDate > lastVisit)
                                    {
                                        output.PostElement.SetHtmlContent(
                                            $"<span class=\"badge badge-primary ml-2\">{T["New"].Value}</span>");
                                        suppressAlterations = true;
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
