using System;
using System.Threading.Tasks;
using Plato.Features.ViewModels;
using PlatoCore.Layout.ViewProviders.Abstractions;

namespace Plato.Features.ViewProviders
{
    
    public class AdminViewProvider : ViewProviderBase<FeaturesIndexViewModel>
    {

        public override Task<IViewProviderResult> BuildDisplayAsync(FeaturesIndexViewModel model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(FeaturesIndexViewModel indexViewModel, IViewProviderContext updater)
        {
            return Task.FromResult(Views(
                View<FeaturesIndexViewModel>("Admin.Index.Header", model => indexViewModel).Zone("header"),
                View<FeaturesIndexViewModel>("Admin.Index.Tools", model => indexViewModel).Zone("header-right"),
                View<FeaturesIndexViewModel>("Admin.Index.Content", model => indexViewModel).Zone("content")
            ));
        }

        public override Task<IViewProviderResult> BuildEditAsync(FeaturesIndexViewModel model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(FeaturesIndexViewModel model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }

}
