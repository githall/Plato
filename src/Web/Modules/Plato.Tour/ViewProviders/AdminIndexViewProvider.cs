using System.Threading.Tasks;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Admin.Models;
using Plato.Tour.ViewModels;
using PlatoCore.Stores.Abstractions.Tour;
using Microsoft.Extensions.Options;
using PlatoCore.Models.Tour;

namespace Plato.Tour.ViewProviders
{

    public class AdminIndexViewProvider : ViewProviderBase<AdminIndex>
    {
        private readonly ITourDescriptorStore _tourDescriptorStore;

        private readonly TourOptions _tourOptions;

        public AdminIndexViewProvider(
             ITourDescriptorStore tourDescriptorStore,
             IOptions<TourOptions> tourOptions)
        {
            _tourDescriptorStore = tourDescriptorStore;
            _tourOptions = tourOptions.Value;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(AdminIndex viewModel,
            IViewProviderContext context)
        {

            if (_tourOptions.Completed)
            {
                return default(IViewProviderResult);
            }

            var descriptor = await _tourDescriptorStore.GetAsync();

            // Build view model
            var coreIndexViewModel = new TourIndexViewModel()
            {
                Steps = descriptor.Steps
            };

            // Build view
            return Views(
                View<TourIndexViewModel>("Tour.Index.Content", model => coreIndexViewModel)
                    .Zone("content").Order(int.MinValue + 5)
            );


        }

        public override Task<IViewProviderResult> BuildDisplayAsync(AdminIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(AdminIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(AdminIndex viewModel,
            IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
    }
    
}
