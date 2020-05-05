using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Plato.Features.ViewModels;
using Plato.Tour.ViewModels;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Tour;
using PlatoCore.Stores.Abstractions.Tour;

namespace Plato.Tour.ViewComponents
{

    public class TourProgressViewComponent : ViewComponent
    {


        private readonly ITourDescriptorStore _tourDescriptorStore;

        private readonly TourOptions _tourOptions;

        public TourProgressViewComponent(
            ITourDescriptorStore tourDescriptorStore,
            IOptions<TourOptions> tourOptions)
        {
            _tourDescriptorStore = tourDescriptorStore;
            _tourOptions = tourOptions.Value;
        }


        public async Task<IViewComponentResult> InvokeAsync(FeatureIndexOptions options)
        {

            var descriptor = await _tourDescriptorStore.GetAsync();

            // Build view model
            var coreIndexViewModel = new TourIndexViewModel()
            {
                Steps = descriptor.Steps
            };

            return View(new TourIndexViewModel()
            {
                Steps = descriptor.Steps
            });

        }
        
    }

}
