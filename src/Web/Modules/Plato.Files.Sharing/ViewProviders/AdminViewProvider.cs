using System.Threading.Tasks;
using Plato.Files.Models;
using Plato.Files.Sharing.ViewModels;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;

namespace Plato.Files.Sharing.ViewProviders
{
    public class AdminViewProvider : ViewProviderBase<File>
    {

        private readonly IContextFacade _contextFacade;   

        public AdminViewProvider(IContextFacade contextFacade)
        {   
            _contextFacade = contextFacade;        
        }

        public override Task<IViewProviderResult> BuildIndexAsync(File file, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(File file, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(File file, IViewProviderContext context)
        {

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Add Files
            ShareFileDialogViewModel viewModel = null;
            if (file.Id == 0)
            {
                return default(IViewProviderResult);
            }

            // Edit File

            viewModel = new ShareFileDialogViewModel()
            {              
                File = file
            };

            return Views(                
                View<ShareFileDialogViewModel>("Admin.ShareFile.Sidebar", model => viewModel).Zone("content-right").Order(10)                
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(File file, IViewProviderContext context)
        {
            return await BuildEditAsync(file, context);
        }

    }

}
