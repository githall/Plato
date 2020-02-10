using System.Threading.Tasks;
using Plato.Core.Models;
using PlatoCore.Layout.ViewProviders.Abstractions;

namespace Plato.Search.ViewProviders
{
    public class HomeViewProvider : ViewProviderBase<HomeIndex>
    {

        public override Task<IViewProviderResult> BuildIndexAsync(HomeIndex viewModel, IViewProviderContext context)
        {

            return Task.FromResult(Views(
                View<HomeIndex>("Core.Search.Index.Tools", model => viewModel)
                    .Zone("tools").Order(int.MinValue)
            ));

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
            return Task.FromResult(default(IViewProviderResult));
        }

    }
    
}
