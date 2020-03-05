using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlatoCore.Data.Abstractions;
using PlatoCore.Navigation.Abstractions;
using Plato.Attachments.Models;
using Plato.Attachments.Stores;
using Plato.Attachments.ViewModels;

namespace Plato.Reports.PageViews.ViewComponents
{
    public class AttachmentListViewComponent : ViewComponent
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

        private readonly IList<SortColumn> _defaultSortColumns = new List<SortColumn>()
        {
            new SortColumn()
            {
                Text = "Created Date",
                Value = SortBy.Created
            },
            new SortColumn()
            {
                Text = "Title",
                Value =  SortBy.Title
            },
            new SortColumn()
            {
                Text = "Url",
                Value = SortBy.Url
            },
            new SortColumn()
            {
                Text = "IP Address",
                Value =  SortBy.IpV4Address
            },
            new SortColumn()
            {
                Text = "User Agent",
                Value =  SortBy.UserAgent
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

        private readonly IAttachmentStore<Attachment> _metricStore;

        public AttachmentListViewComponent(
            IAttachmentStore<Attachment> metricStore)
        {
            _metricStore = metricStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            AttachmentIndexOptions options,
            PagerOptions pager)
        {

            if (options == null)
            {
                options = new AttachmentIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }

            return View(await GetViewModel(options, pager));

        }

        async Task<AttachmentIndexViewModel> GetViewModel(
            AttachmentIndexOptions options,
            PagerOptions pager)
        {

            // Get results
            var results = await _metricStore.QueryAsync()
                .Take(pager.Page, pager.Size, pager.CountTotal)
                .Select<AttachmentQueryParams>(q =>
                {
                    //q.StartDate.GreaterThanOrEqual(options.Start);
                    //q.EndDate.LessThanOrEqual(options.End);
                })
                .OrderBy(options.Sort.ToString(), options.Order)
                .ToList();
              
            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new AttachmentIndexViewModel()
            {
                SortColumns = _defaultSortColumns,
                SortOrder = _defaultSortOrder,
                Filters = _defaultFilters,
                Results = results,
                Options = options,
                Pager = pager
            };

        }

    }

}
