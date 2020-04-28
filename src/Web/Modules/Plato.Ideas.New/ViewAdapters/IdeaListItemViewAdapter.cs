using System;
using System.Linq;
using System.Threading.Tasks;
using Plato.Ideas.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Localization;
using PlatoCore.Layout.TagHelperAdapters.Abstractions;
using PlatoCore.Abstractions.Extensions;
using Plato.Entities.Metrics.Repositories;
using Plato.Entities.Metrics.Extensions;
using Plato.Entities.Extensions;
using PlatoCore.Hosting.Web.Abstractions;

namespace Plato.Ideas.New.ViewAdapters
{

    public class IdeaListItemViewAdapter : ViewAdapterProviderBase
    {

        private readonly IAggregatedEntityMetricsRepository _agggregatedEntityMetricsRepository;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IContextFacade _contextFacade;      

        public IHtmlLocalizer T { get; }

        public IdeaListItemViewAdapter(
            IHtmlLocalizer<IdeaListItemViewAdapter> localizer,
            IAggregatedEntityMetricsRepository agggregatedEntityMetricsRepository,    
            IActionContextAccessor actionContextAccessor,         
            IContextFacade contextFacade)
        {

            _agggregatedEntityMetricsRepository = agggregatedEntityMetricsRepository;      
            _actionContextAccessor = actionContextAccessor;        
            _contextFacade = contextFacade; 

            T = localizer;
            ViewName = "IdeaListItem";

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
                v.AdaptModel<EntityListItemViewModel<Idea>>(async model =>
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
                        var viewModel = _actionContextAccessor.ActionContext.HttpContext.Items[typeof(EntityIndexViewModel<Idea>)] as EntityIndexViewModel<Idea>;
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

                    // Build tag adapters
                    var adapters = new[]
                    {
                        // Add a "New" label after the title tag 
                        new TagHelperAdapter(ViewName, "title", (context, output) =>
                        {
                            if (lastVisit != null)
                            {
                                // New
                                if (model.Entity.LastReplyAfter(lastVisit))
                                {
                                    output.PreElement.SetHtmlContent(
                                        $"<span class=\"text-primary mr-2 smaller\" data-toggle=\"tooltip\" title=\"{T["This idea has new replies"].Value}\"><i class=\"fa fa-circle\"></i></span>");
                                }
                                else
                                {
                                    // Modified
                                    if (model.Entity.ModifiedAfter(lastVisit))
                                    {
                                        output.PreElement.SetHtmlContent(
                                            $"<span class=\"text-primary mr-2 smaller\" data-toggle=\"tooltip\" title=\"{T["This idea has been updated since it was last read"].Value}\"><i class=\"fa fa-circle\"></i></span>");
                                    }
                                }
                            }
                            else
                            {
                                // Unread
                                output.PreElement.SetHtmlContent(
                                    $"<span data-toggle=\"tooltip\" title=\"{T["You've not read this idea"].Value}\" class=\"text-primary mr-2 smaller\"><i class=\"fa fa-circle\"></i></span>");
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
