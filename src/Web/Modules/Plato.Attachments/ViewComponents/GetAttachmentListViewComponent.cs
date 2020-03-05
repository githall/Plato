using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlatoCore.Navigation.Abstractions;
using Plato.Attachments.Stores;
using Plato.Attachments.Models;
using Plato.Attachments.ViewModels;

namespace Plato.Attachments.ViewComponents
{
    public class GetAttachmentListViewComponent : ViewComponent
    {

        private readonly IAttachmentStore<Attachment> _metricStore;

        public GetAttachmentListViewComponent(IAttachmentStore<Attachment> metricStore)
        {
            _metricStore = metricStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(AttachmentIndexOptions options, PagerOptions pager)
        {

            // Build default
            if (options == null)
            {
                options = new AttachmentIndexOptions();
            }

            // Build default
            if (pager == null)
            {
                pager = new PagerOptions();
            }
        
            // Review view
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
            return new AttachmentIndexViewModel
            {
                Results = results,
                Options = options,
                Pager = pager
            };

        }

    }
    
}

