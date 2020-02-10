using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Docs.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Docs.ViewComponents
{
    public class DocCommentListItemViewComponent : ViewComponentBase
    {

        public Task<IViewComponentResult> InvokeAsync(
            EntityReplyListItemViewModel<Doc, DocComment> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}



