using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Ideas.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Ideas.ViewComponents
{
    public class IdeaCommentListItemViewComponent : ViewComponentBase
    {
        
        public Task<IViewComponentResult> InvokeAsync(EntityReplyListItemViewModel<Idea, IdeaComment> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}


