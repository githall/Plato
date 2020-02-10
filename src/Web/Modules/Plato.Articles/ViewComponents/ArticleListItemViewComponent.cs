using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Articles.ViewComponents
{
    public class ArticleListItemViewComponent : ViewComponentBase
    {
        
        public ArticleListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(
            EntityListItemViewModel<Article> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

