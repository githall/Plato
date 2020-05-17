using System.Threading.Tasks;
using Plato.Docs.Models;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Docs.Anchorific.ViewModels;

namespace Plato.Docs.Anchorific.ViewProviders
{

    public class DocViewProvider : ViewProviderBase<Doc>
    {

        public DocViewProvider()
        {         
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Doc entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Doc entity, IViewProviderContext updater)
        {

            if (entity == null)
            {
                return await BuildIndexAsync(new Doc(), updater);
            }

            var viewModel = new AnchorificViewModel();

            return Views(
                View<AnchorificViewModel>("Doc.Anchorific.Asides", model => viewModel)
                .Zone("content-right").Order(-4)
            );

        }

        public override Task<IViewProviderResult> BuildEditAsync(Doc entity, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Doc article, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
