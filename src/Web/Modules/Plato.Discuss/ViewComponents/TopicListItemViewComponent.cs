using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Discuss.ViewComponents
{
    public class TopicListItemViewComponent : BaseViewComponent
    {

        public TopicListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(EntityListItemViewModel<Topic> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}

