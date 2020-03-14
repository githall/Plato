using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlatoCore.Navigation.Abstractions;
using Plato.Files.Stores;
using Plato.Files.Models;
using Plato.Files.ViewModels;

namespace Plato.Files.ViewComponents
{
    public class GetFileListViewComponent : ViewComponent
    {

        private readonly IFileStore<File> _metricStore;

        public GetFileListViewComponent(IFileStore<File> metricStore)
        {
            _metricStore = metricStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(FileIndexOptions options, PagerOptions pager)
        {

            // Build default
            if (options == null)
            {
                options = new FileIndexOptions();
            }

            // Build default
            if (pager == null)
            {
                pager = new PagerOptions();
            }
        
            // Review view
            return View(await GetViewModel(options, pager));

        }

        async Task<FileIndexViewModel> GetViewModel(
            FileIndexOptions options,
            PagerOptions pager)
        {

            // Get results
            var results = await _metricStore.QueryAsync()
                .Take(pager.Page, pager.Size, pager.CountTotal)
                .Select<FileQueryParams>(q =>
                {
                    //q.StartDate.GreaterThanOrEqual(options.Start);
                    //q.EndDate.LessThanOrEqual(options.End);
                })
                .OrderBy(options.Sort.ToString(), options.Order)
                .ToList();

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new FileIndexViewModel
            {
                Results = results,
                Options = options,
                Pager = pager
            };

        }

    }
    
}

