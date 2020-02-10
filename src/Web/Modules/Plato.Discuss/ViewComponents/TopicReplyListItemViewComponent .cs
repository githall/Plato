using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Discuss.ViewComponents
{
    public class TopicReplyListItemViewComponent : ViewComponentBase
    {
        
        public Task<IViewComponentResult> InvokeAsync(
            EntityReplyListItemViewModel<Topic, Reply> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

