using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Issues.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Issues.ViewComponents
{
    public class IssueCommentListItemViewComponent : ViewComponentBase
    {
        
        public Task<IViewComponentResult> InvokeAsync(
            EntityReplyListItemViewModel<Issue, Comment> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}



