using System.Threading.Tasks;
using Plato.Core.Models;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Tour.ViewModels;
using PlatoCore.Models.Tour;
using PlatoCore.Stores.Abstractions.Tour;

namespace Plato.Tour.ViewProviders
{
    public class HomeViewProvider : ViewProviderBase<HomeIndex>
    {

        private readonly ITourDescriptorStore _tourDescriptorStore;

        public HomeViewProvider(
            ITourDescriptorStore tourDescriptorStore)
        {
            _tourDescriptorStore = tourDescriptorStore;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(HomeIndex viewModel, IViewProviderContext context)
        {

            var descriptor = await _tourDescriptorStore.GetAsync();

            // Build view model
            var coreIndexViewModel = new TourIndexViewModel()
            {
                Steps = descriptor.Steps
            };

            // Build view
            return Views(
                View<TourIndexViewModel>("Tour.Index.Content", model => coreIndexViewModel)
                    .Zone("content")
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(HomeIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(HomeIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(HomeIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult)); ;
        }

    }

}
