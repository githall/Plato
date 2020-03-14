using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Entities.Files.ViewModels;
using Plato.Files.Models;
using Plato.Files.Stores;
using Plato.Files.ViewModels;
using PlatoCore.Layout.ViewProviders.Abstractions;

namespace Plato.Entities.Files.ViewProviders
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
            return Task.FromResult(default(IViewProviderResult));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(File file, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(File file, IViewProviderContext context)
        {

            var viewModel = new FileEntitiesViewModel();

            return Task.FromResult(Views(
                
                View<FileEntitiesViewModel>("Admin.Edit.FileEntities", model => viewModel).Zone("sidebar").Order(5)
                
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
