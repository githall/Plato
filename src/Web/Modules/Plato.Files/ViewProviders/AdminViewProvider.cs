using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Files.Models;
using Plato.Files.Stores;
using Plato.Files.ViewModels;
using PlatoCore.Layout.ViewProviders.Abstractions;

namespace Plato.Files.ViewProviders
{
    public class AdminViewProvider : ViewProviderBase<File>
    {

        private readonly IFileStore<File> _fileStore;

        public AdminViewProvider(
            IFileStore<File> fileStore)
        {
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

        public override Task<IViewProviderResult> BuildEditAsync(File file, IViewProviderContext context)
        {
            EditFileViewModel viewModel = null;
            if (file.Id == 0)
            {
                viewModel = new EditFileViewModel()
                {
                    IsNewFile = true,
                    PostRoute = new RouteValueDictionary()
                    {
                        ["area"] = "Plato.Files",
                        ["controller"] = "Api",
                        ["action"] = "post"
                    }
                };
            }
            else
            {

                viewModel = new EditFileViewModel()
                {
                    File = file,
                    PostRoute = new RouteValueDictionary()
                    {
                        ["area"] = "Plato.Files",
                        ["controller"] = "Api",
                        ["action"] = "put",
                        ["id"] = file.Id.ToString()
                    },
                    ShareRoute = new RouteValueDictionary()
                    {
                        ["area"] = "Plato.Files",
                        ["controller"] = "Home",
                        ["action"] = "Share",
                        ["id"] = file.Id,
                        ["checkSum"] = file.ContentCheckSum
                    }
                };
            }

            return Task.FromResult(Views(
                View<EditFileViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<EditFileViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1),
                View<EditFileViewModel>("Admin.Edit.Sidebar", model => viewModel).Zone("sidebar").Order(1),
                View<EditFileViewModel>("Admin.Edit.Actions", model => viewModel).Zone("actions").Order(1),
                View<EditFileViewModel>("Admin.Edit.Footer", model => viewModel).Zone("footer").Order(1)
            ));

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(File file, IViewProviderContext context)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            //if (file.IsNewFile)
            //{
            //    return await BuildEditAsync(file, context);
            //}

            var model = new EditFileViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(file, context);
            }

            model.Name = model.Name?.Trim();

            //model.Description = model.Description?.Trim();

            if (context.Updater.ModelState.IsValid)
            {

                file.Name = model.Name;
                //file.Description = model.Description;
                //file.ForeColor = model.ForeColor;
                //file.BackColor = model.BackColor;

                var updatedFile = await _fileStore.UpdateAsync(file);

                //foreach (var error in result.Errors)
                //{
                //    context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                //}

            }

            return await BuildEditAsync(file, context);

        }

    }

}
