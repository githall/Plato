using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using PlatoCore.Data.Abstractions;
using PlatoCore.Layout.Views.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Entities.ViewComponents
{
    public class EntityListViewComponent : ViewComponentBase
    {


        private readonly IList<Filter> _defaultFilters = new List<Filter>()
        {
            new Filter()
            {
                Text = "All",
                Value = FilterBy.All
            },
            new Filter()
            {
                Text = "-" // represents menu divider
            },
            new Filter()
            {
                Text = "My Topics",
                Value = FilterBy.Started
            },
            new Filter()
            {
                Text = "Participated",
                Value = FilterBy.Participated
            },
            new Filter()
            {
                Text = "Following",
                Value = FilterBy.Following
            },
            new Filter()
            {
                Text = "Starred",
                Value = FilterBy.Starred
            },
            new Filter()
            {
                Text = "-"  // represents menu divider
            },
            new Filter()
            {
                Text = "Unanswered",
                Value = FilterBy.Unanswered
            },
            new Filter()
            {
                Text = "No Replies",
                Value = FilterBy.NoReplies
            }
        };

        private readonly ICollection<SortColumn> _defaultSortColumns = new List<SortColumn>()
        {
            new SortColumn()
            {
                Text = "Latest",
                Value = SortBy.LastReply
            },
            new SortColumn()
            {
                Text = "Popular",
                Value = SortBy.Popular
            },
            new SortColumn()
            {
                Text = "-" // represents menu divider
            },
            new SortColumn()
            {
                Text = "Replies",
                Value =  SortBy.Replies
            },
            new SortColumn()
            {
                Text = "Views",
                Value = SortBy.Views
            },
            new SortColumn()
            {
                Text = "Participants",
                Value =  SortBy.Participants
            },
            new SortColumn()
            {
                Text = "Reactions",
                Value =  SortBy.Reactions
            },
            new SortColumn()
            {
                Text = "Follows",
                Value =  SortBy.Follows
            },
            new SortColumn()
            {
                Text = "Stars",
                Value =  SortBy.Stars
            },
            new SortColumn()
            {
                Text = "Created",
                Value = SortBy.Created
            },
            new SortColumn()
            {
                Text = "Modified",
                Value = SortBy.Modified
            }
        };

        private readonly IList<SortOrder> _defaultSortOrder = new List<SortOrder>()
        {
            new SortOrder()
            {
                Text = "Descending",
                Value = OrderBy.Desc
            },
            new SortOrder()
            {
                Text = "Ascending",
                Value = OrderBy.Asc
            },
        };

        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityService<Entity> _entityService;
    
        public EntityListViewComponent(
            IEntityService<Entity> entityService,
            IAuthorizationService authorizationService)
        {            
            _authorizationService = authorizationService;
            _entityService = entityService;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityIndexViewModel<Entity> model)
        {

            if (model == null)
            {
                model = new EntityIndexViewModel<Entity>();
            }

            if (model.Options == null)
            {
                model.Options = new EntityIndexOptions();
            }

            if (model.Pager == null)
            {
                model.Pager = new PagerOptions();
            }

            // Return view model
            return View(await GetIndexViewModel(model));

        }
        
        async Task<EntityIndexViewModel<Entity>> GetIndexViewModel(EntityIndexViewModel<Entity> model)
        {

            // If full text is enabled add rank to sort options
            if (model.Options.Sort == SortBy.Rank)
            {
                var sortColumns = new List<SortColumn>()
                {
                    new SortColumn()
                    {
                        Text = "Relevancy",
                        Value = SortBy.Rank
                    }
                };
                sortColumns.AddRange(model.SortColumns);
                model.SortColumns = sortColumns;
            }

            // Build results
            var results = await _entityService
                .ConfigureQuery(async q =>
                {

                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewPrivateEntities))
                    {
                        q.HidePrivate.True();
                    }

                    // Hide hidden?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewHiddenEntities))
                    {
                        q.HideHidden.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewSpamEntities))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewDeletedEntities))
                    {
                        q.HideDeleted.True();
                    }

                })
                .GetResultsAsync(model.Options, model.Pager);

            // Set pager total
            model.Pager.SetTotal(results?.Total ?? 0);

            model.Results = results;
            model.SortColumns = _defaultSortColumns;
            model.SortOrder = _defaultSortOrder;
            model.Filters = _defaultFilters;

            return model;

        }
    
    }
    
}

