using System.Threading.Tasks;
using Plato.Files.Models;
using Plato.Files.Services;
using Plato.Files.Sharing.ViewModels;
using Plato.Files.Stores;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;

namespace Plato.Files.Sharing.ViewProviders
{
    public class AdminViewProvider : ViewProviderBase<File>
    {
  
        private readonly IFileOptionsFactory _fileOptionsFactory;
        private readonly IFileInfoStore<FileInfo> _fileInfoStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFileStore<File> _fileStore;

        public AdminViewProvider(
            IFileOptionsFactory fileOptionsFactory,
            IFileInfoStore<FileInfo> fileInfoStore,
            IContextFacade contextFacade,
            IFileStore<File> fileStore)
        {
            _fileOptionsFactory = fileOptionsFactory;
            _fileInfoStore = fileInfoStore;
            _contextFacade = contextFacade;
            _fileStore = fileStore;
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
                View<ShareFileDialogViewModel>("Admin.ShareFile.Sidebar", model => viewModel).Zone("sidebar").Order(10)                
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(File file, IViewProviderContext context)
        {
            return await BuildEditAsync(file, context);
        }

    }

}
