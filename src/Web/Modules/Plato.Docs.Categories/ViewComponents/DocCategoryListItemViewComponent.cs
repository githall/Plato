using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.ViewModels;
using Plato.Docs.Categories.Models;
using Plato.Docs.Categories.ViewModels;
using PlatoCore.Hosting.Web.Abstractions;

namespace Plato.Docs.Categories.ViewComponents
{

    public class DocCategoryListItemViewComponent : ViewComponent
    {
 
        public DocCategoryListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(Category category, CategoryIndexOptions options)
        {

            if (options == null)
            {
                options = new CategoryIndexOptions();
            }

            var model = new CategoryListItemViewModel<Category>()
            {
                Category = category,
                Options = options
            };

            return Task.FromResult((IViewComponentResult)View(model));

        }


    }


}
