using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Reports.ViewModels;
using PlatoCore.Hosting.Abstractions;
using Plato.Metrics.Repositories;

namespace Plato.Reports.TopPages.ViewComponents
{
    public class PageViewsTopListViewComponent : ViewComponent
    {
        
        private readonly IAggregatedMetricsRepository _aggregatedMetricsRepository;
        private readonly IContextFacade _contextFacade;

        public PageViewsTopListViewComponent(
            IAggregatedMetricsRepository aggregatedMetricsRepository,
            IContextFacade contextFacade)
        {
            _aggregatedMetricsRepository = aggregatedMetricsRepository;
            _contextFacade = contextFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            ReportOptions options,
            ChartOptions chart)
        {
            
            if (options == null)
            {
                options = new ReportOptions();
            }

            if (chart == null)
            {
                chart = new ChartOptions();
            }
            
            var results = await _aggregatedMetricsRepository.SelectGroupedByStringAsync(
                "[Url]",
                options.Start,
                options.End,
                100);

            if (results != null)
            {
                var url = await _contextFacade.GetBaseUrlAsync();
                foreach (var result in results.Data)
                {
                    result.Aggregate = result.Aggregate?.ToLower().Replace(url.ToLower(), "");
                }
            }

            return View(results);

        }

    }

}
