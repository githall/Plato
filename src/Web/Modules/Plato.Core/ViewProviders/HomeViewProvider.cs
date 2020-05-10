using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Core.Models;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Core.ViewProviders
{
    public class HomeViewProvider : ViewProviderBase<HomeIndex>
    {

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly INavigationManager _navigationManager;

        public HomeViewProvider(
            IActionContextAccessor actionContextAccessor,
            INavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            _actionContextAccessor = actionContextAccessor;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(HomeIndex viewModel, IViewProviderContext context)
        {

            var actionContext = _actionContextAccessor.ActionContext;
            var items = _navigationManager.BuildMenu("home", actionContext);

            if (items.Any())
            {
                return Task.FromResult(Views(
                    View<HomeIndex>("Home.Index.Header", model => viewModel).Zone("header"),
                    View<HomeIndex>("Home.Index.Tools", model => viewModel).Zone("header-right"),
                    View<HomeIndex>("Home.Index.Content", model => viewModel).Zone("content")
                ));
            }

            return Task.FromResult(Views(
                View<HomeIndex>("Home.Index.Header", model => viewModel).Zone("header"),
                View<HomeIndex>("Home.Index.Tools", model => viewModel).Zone("header-right")
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
