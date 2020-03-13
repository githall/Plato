using System;
using System.Threading.Tasks;
using Plato.Files.Models;
using Plato.Files.ViewModels;
using PlatoCore.Layout.ViewProviders.Abstractions;

namespace Plato.Files.ViewProviders
{
    public class FileIndexViewProvider : ViewProviderBase<AdminFilesIndex>
    {

        public override Task<IViewProviderResult> BuildIndexAsync(AdminFilesIndex model, IViewProviderContext context)
        {

            // Get view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(AttachmentIndexViewModel)] as AttachmentIndexViewModel;

            // Ensure we have the view model
            if (viewModel == null)
            {
                throw new Exception($"No type of \"{typeof(AttachmentIndexViewModel)}\" has been registered with HttpContext.Items");
            }

            return Task.FromResult(Views(
                View<AttachmentIndexViewModel>("Admin.Index.Header", model => viewModel).Zone("header").Order(1),
                View<AttachmentIndexViewModel>("Admin.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<AttachmentIndexViewModel>("Admin.Index.Content", model => viewModel).Zone("content").Order(1)
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(AdminFilesIndex model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(AdminFilesIndex model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(AdminFilesIndex model, IViewProviderContext context)
        {           
            return await BuildEditAsync(model, context);
        }

    }

}
