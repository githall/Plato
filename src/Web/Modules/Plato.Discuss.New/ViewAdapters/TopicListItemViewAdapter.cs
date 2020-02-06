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
using PlatoCore.Models.Metrics;
using PlatoCore.Abstractions.Extensions;

namespace Plato.Discuss.New.ViewAdapters
{

    public class TopicListItemViewAdapter : BaseAdapterProvider
    {

        private readonly IDbHelper _dbHelper;
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
            IFeatureFacade featureFacade,
            IDbHelper dbHelper)
        {
            _actionContextAccessor = actionContextAccessor;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
            _entityMetricsStore = entityMetricsStore;
            _featureFacade = featureFacade;
            _entityService = entityService;
            _dbHelper = dbHelper;
            ViewName = "TopicListItem";
        }

        IDictionary<int, DateTimeOffset?> _lastVisits;

        public override async Task<IViewAdapterResult> ConfigureAsync(string viewName)
        {

            if (!viewName.Equals(ViewName, StringComparison.OrdinalIgnoreCase))
            {
                return default(IViewAdapterResult);
            }

            if (_lastVisits == null)
            {
                // Get displayed entities
                var entities = await GetDisplayedEntitiesAsync();
                if (entities?.Data != null)
                {
                    _lastVisits = await SelectLatestViewDateForEntitiesAsync(entities.Data.Select(e => e.Id).ToArray());
                }
            }

            if (_lastVisits == null)
            {
                // No metrics available to adapt the view
                return default(IViewAdapterResult);
            }

            // Adapt model
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

                    DateTimeOffset? lastVisit = null;
                    if (_lastVisits.ContainsKey(model.Entity.Id))
                    {
                        lastVisit = _lastVisits[model.Entity.Id];
                    }

                    // Ensure tag alterations
                    if (model.TagAlterations == null)
                    {
                        model.TagAlterations = new TagAlterations();
                    }

                    // Build tag alterations
                    var alterations = new[]
                    {
                        new TagAlteration("title", (context, output) =>
                        {
                            if (lastVisit != null)
                            {

                                if (model.Entity.LastReplyDate.HasValue)
                                {
                                    if (model.Entity.LastReplyDate > lastVisit)
                                    {
                                        output.PostContent.SetHtmlContent(
                                            "<span class=\"badge badge-primary\">NEW</span>");
                                    }
                                }
                                else
                                {
                                    if (model.Entity.CreatedDate > lastVisit)
                                    {
                                        output.PostContent.SetHtmlContent(
                                            "<span class=\"badge badge-primary\">NEW</span>");
                                    }
                                }

                            }
                            else
                            {
                                output.PostContent.SetHtmlContent("<span class=\"badge badge-primary\">NEW</span>");
                            }
                        })
                    };

                    // Apply tag alterations
                    model.TagAlterations.Add(alterations);

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

        public async Task<IDictionary<int, DateTimeOffset?>> SelectLatestViewDateForEntitiesAsync(int[] ids)
        {

            const string sql = @"                
                SELECT 
                    em.EntityId AS EntityId, 
                    MAX(em.CreatedDate) AS CreatedDate
                FROM 
                     {prefix}_EntityMetrics em
                WHERE
                    em.EntityId IN ({ids})
                GROUP BY (em.EntityId)
            ";

            // Sql replacements
            var replacements = new Dictionary<string, string>()
            {
                ["{ids}"] = ids.ToDelimitedString()
            };

            // Execute and return results
            return await _dbHelper.ExecuteReaderAsync(sql, replacements, async dr =>
            {
                var output = new Dictionary<int, DateTimeOffset?>();
                while (await dr.ReadAsync())
                {

                    var key = 0;
                    DateTimeOffset? value = null;

                    if (dr.ColumnIsNotNull("EntityId"))
                        key = Convert.ToInt32((dr["EntityId"]));

                    if (dr.ColumnIsNotNull("CreatedDate"))
                        value = (DateTimeOffset) (dr["CreatedDate"]);

                    if (!output.ContainsKey(key))
                    {
                        output[key] = value;
                    }

                }

                return output;
            });

        }

    }

}
