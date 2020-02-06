using System;
using System.Linq;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout.ViewAdapters;
using System.Collections.Generic;
using Plato.Entities.Metrics.Stores;
using Plato.Entities.Metrics.Models;
using PlatoCore.Models;
using PlatoCore.Data.Abstractions;
using Plato.Entities.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using PlatoCore.Security.Abstractions;
using PlatoCore.Layout.Models;

namespace Plato.Discuss.New.ViewAdapters
{

    public class TopicListItemViewAdapter : BaseAdapterProvider
    {

        private readonly IEntityMetricsStore<EntityMetric> _entityMetricsStore;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IAuthorizationService _authorizationService;        
        private readonly IHttpContextAccessor _httpContextAccessor;    
        private readonly IEntityService<Topic> _entityService;
        private readonly IFeatureFacade _featureFacade;

        public TopicListItemViewAdapter(
            IEntityMetricsStore<EntityMetric> entityMetricsStore,
            IActionContextAccessor actionContextAccessor,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IEntityService<Topic> entityService,
            IFeatureFacade featureFacade)
        {
            _actionContextAccessor = actionContextAccessor;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
            _entityMetricsStore = entityMetricsStore;
            _featureFacade = featureFacade;
            _entityService = entityService;
            ViewName = "TopicListItem";
        }

        IEnumerable<EntityMetric> _metrics;

        public override async Task<IViewAdapterResult> ConfigureAsync(string viewName)
        {

            if (!viewName.Equals(ViewName, StringComparison.OrdinalIgnoreCase))
            {
                return default(IViewAdapterResult);
            }

            //if (_metrics == null)
            //{

            //    // Get displayed entities
            //    var entities = await GetDisplayedEntitiesAsync();

            //    // Get all metrics for displayed entities
            //    var metrics = await _entityMetricsStore.QueryAsync()
            //        .Select<EntityMetricQueryParams>(q =>
            //        {
            //            q.EntityId.IsIn(entities.Data.Select(e => e.Id).ToArray());
            //        })
            //        .OrderBy("CreatedDate", OrderBy.Desc)
            //        .ToList();

            //    if (metrics != null)
            //    {
            //        _metrics = metrics.Data;
            //    }
            //}

            //if (_metrics == null)
            //{
            //    // No categories available to adapt the view 
            //    //return default(IViewAdapterResult);
            //}

            // Plato.Discuss does not have a dependency on Plato.Discuss.Categories
            // Instead we update the model for the topic item view component
            // here via our view adapter to include the channel information
            // This way the channel data is only ever populated if the channels feature is enabled
            return await Adapt(ViewName, v =>
            {
                v.AdaptModel<EntityListItemViewModel<Topic>>(model =>
                  {

                      if (model.Entity == null)
                      {
                          // Return an anonymous type, we are adapting a view component
                          return new
                          {
                              model
                          };
                      }

                      if (model.TagAlterations == null)
                      {
                          model.TagAlterations = new TagAlterations();
                      }

                      model.TagAlterations.Add(new TagAlteration("avatar", (context, output) =>
                      {
                          if (model.Labels != null)
                          {
                              output.PostContent.SetHtmlContent("<span class=\"badge badge-primary\">NEW</span>");
                          }                          
                      }));

                      //// Get our category
                      ///
                      //var category = _metrics.FirstOrDefault(c => c.Id == model.Entity.CategoryId);
                      //if (category != null)
                      //{
                      //    model.Parts.Add(new HtmlPart()
                      //    {
                      //        Id = "title:before"
                      //    });
                      //}

                      // Return an anonymous type, we are adapting a view component
                      return new
                      {
                          model
                      };

                  });

            });

        }

        private async Task<IPagedResults<Topic>> GetDisplayedEntitiesAsync()
        {

            // Get topic index view model from context
            var viewModel = _actionContextAccessor.ActionContext.HttpContext.Items[typeof(EntityIndexViewModel<Topic>)] as EntityIndexViewModel<Topic>;
            if (viewModel == null)
            {
                return null;
            }

            // Get all entities for our current view
            return await _entityService
                .ConfigureQuery(async q =>
                {

                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User,
                        Permissions.ViewPrivateTopics))
                    {
                        q.HidePrivate.True();
                    }

                    // Hide hidden?
                    if (!await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User,
                        Permissions.ViewHiddenTopics))
                    {
                        q.HideHidden.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User,
                        Permissions.ViewSpamTopics))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User,
                        Permissions.ViewDeletedTopics))
                    {
                        q.HideDeleted.True();
                    }

                })
                .GetResultsAsync(viewModel?.Options, viewModel?.Pager);

        }

    }

}
