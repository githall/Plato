using System.Threading.Tasks;
using Plato.Admin.Models;
using Plato.Admin.ViewModels;
using PlatoCore.Layout.ViewProviders.Abstractions;

namespace Plato.Admin.ViewProviders
{

    public class AdminViewProvider : ViewProviderBase<AdminIndex>
    {

        public override Task<IViewProviderResult> BuildIndexAsync(AdminIndex viewModel, IViewProviderContext context)
        {

            var adminViewModel = new AdminViewModel();
            return Task.FromResult(Views(
                View<AdminViewModel>("Admin.Index.Header", model => adminViewModel).Zone("header").Order(1),
                View<AdminViewModel>("Admin.Index.Tools", model => adminViewModel).Zone("header-right").Order(1),
                View<AdminViewModel>("Admin.Index.Content", model => adminViewModel).Zone("content").Order(1)
            ));
            
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(AdminIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(AdminIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(AdminIndex viewModel, IViewProviderContext context)
        {
            return await BuildEditAsync(viewModel, context);
        }

    }

}
