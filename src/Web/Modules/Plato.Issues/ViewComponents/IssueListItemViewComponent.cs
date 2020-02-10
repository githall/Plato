using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Issues.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Issues.ViewComponents
{
    public class IssueListItemViewComponent : ViewComponentBase
    {
        
        public IssueListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(
            EntityListItemViewModel<Issue> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

