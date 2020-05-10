using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Files.Models;
using Plato.Files.Services;
using Plato.Files.Stores;
using Plato.Files.ViewModels;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;

namespace Plato.Files.ViewProviders
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

            // Get view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(FileIndexViewModel)] as FileIndexViewModel;

            // Ensure we have the view model
            if (viewModel == null)
            {
                throw new Exception($"No type of \"{typeof(FileIndexViewModel)}\" has been registered with HttpContext.Items");
            }

            return Task.FromResult(Views(
                View<FileIndexViewModel>("Admin.Index.Header", model => viewModel).Zone("header").Order(1),
                View<FileIndexViewModel>("Admin.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<FileIndexViewModel>("Admin.Index.Content", model => viewModel).Zone("content").Order(1)
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(File file, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(File file, IViewProviderContext context)
        {

            var user = await _contextFacade.GetAuthenticatedUserAsync();
                        
            // Add Files
            EditFileViewModel viewModel = null;
            if (file.Id == 0)
            {

                viewModel = new EditFileViewModel()
                {
                    Info = await _fileInfoStore.GetByUserIdAsync(user?.Id ?? 0),
                    Options = await _fileOptionsFactory.GetOptionsAsync(user),
                    PostRoute = new RouteValueDictionary()
                    {
                        ["area"] = "Plato.Files",
                        ["controller"] = "Api",
                        ["action"] = "post"
                    },
                    ReturnRoute = new RouteValueDictionary()
                    {
                        ["area"] = "Plato.Files",
                        ["controller"] = "Admin",
                        ["action"] = "Index"
                    }
                };

                return Views(
                    View<EditFileViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                    View<EditFileViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)                                     
                );

            }

            // Edit File

            viewModel = new EditFileViewModel()
            {
                File = file,
                Info = await _fileInfoStore.GetByUserIdAsync(user?.Id ?? 0),
                Options = await _fileOptionsFactory.GetOptionsAsync(user),
                PostRoute = new RouteValueDictionary()
                {
                    ["area"] = "Plato.Files",
                    ["controller"] = "Api",
                    ["action"] = "put",
                    ["id"] = file.Id.ToString()
                },
                ReturnRoute = new RouteValueDictionary()
                {
                    ["area"] = "Plato.Files",
                    ["controller"] = "Admin",
                    ["action"] = "Edit",
                    ["id"] = file.Id.ToString()
                }
            };

            return Views(
                View<EditFileViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<EditFileViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1),
                View<EditFileViewModel>("Admin.Edit.Sidebar", model => viewModel).Zone("content-right").Order(1),                
                View<EditFileViewModel>("Admin.Edit.Footer", model => viewModel).Zone("actions").Order(1),
                View<EditFileViewModel>("Admin.Edit.Actions", model => viewModel).Zone("actions-right").Order(1)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(File file, IViewProviderContext context)
        {
            return await BuildEditAsync(file, context);
        }

    }

}
