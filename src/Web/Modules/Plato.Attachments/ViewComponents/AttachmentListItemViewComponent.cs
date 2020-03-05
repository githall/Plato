using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Attachments.ViewModels;

namespace Plato.Attachments.ViewComponents
{
    public class AttachmentListItemViewComponent : ViewComponent
    {
  
        public AttachmentListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(AttachmentListItemViewModel model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

