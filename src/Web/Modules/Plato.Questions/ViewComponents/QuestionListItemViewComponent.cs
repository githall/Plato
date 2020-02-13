using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Questions.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Questions.ViewComponents
{

    public class QuestionListItemViewComponent : ViewComponentBase
    {

        public Task<IViewComponentResult> InvokeAsync(EntityListItemViewModel<Question> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}

