using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Files.ViewModels;

namespace Plato.Files.ViewComponents
{
    public class FileListItemViewComponent : ViewComponent
    {
  
        public FileListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(FileListItemViewModel model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

