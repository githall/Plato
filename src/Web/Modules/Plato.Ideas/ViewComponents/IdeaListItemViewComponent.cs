using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Ideas.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Ideas.ViewComponents
{
    public class IdeaListItemViewComponent : ViewComponentBase
    {

        public Task<IViewComponentResult> InvokeAsync(EntityListItemViewModel<Idea> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

