using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.ViewModels;
using Plato.Articles.Categories.Models;
using PlatoCore.Navigation.Abstractions;
using System;

namespace Plato.Articles.Categories.ViewComponents
{

    public class ArticleCategoryTreeViewComponent : ViewComponent
    {

        private const string _controllerKey = "controller";

        private readonly ICategoryService<Category> _categoryService;
     
        public ArticleCategoryTreeViewComponent(ICategoryService<Category> categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(CategoryTreeOptions options)
        {

            if (options == null)
            {
                options = new CategoryTreeOptions();
            }

            if (options.SelectedCategories == null)
            {
                options.SelectedCategories = new int[0];
            }

            return View(new CategoryTreeViewModel
            {
                HtmlName = options.HtmlName,
                EnableCheckBoxes = options.EnableCheckBoxes,
                EditMenuViewName = options.EditMenuViewName,
                SelectedCategories = await BuildCategories(options),
                CssClass = options.CssClass,
                RouteValues = options.RouteValues
            });

        }
        

        private async Task<IList<Selection<CategoryBase>>> BuildCategories(CategoryTreeOptions options)
        {

            // Get categories
            var categories = await _categoryService
                .ConfigureQuery(q =>
                {
                    // Skip role based security when
                    // viewing categories within the admin
                    if (IsAdminController())
                    {
                        q.UserId.Clear();
                    }
                })
                .GetResultsAsync(options.IndexOptions, new PagerOptions()
                {
                    Page = 1,
                    Size = int.MaxValue
                });

            return categories?.Data.Select(c => new Selection<CategoryBase>
                {
                    IsSelected = options.SelectedCategories.Any(v => v == c.Id),
                    Value = c
                })
                .ToList();

        }

        bool IsAdminController()
        {

            var routeValues = ViewContext.RouteData.Values;
            var controllerName = string.Empty;
            if (routeValues.ContainsKey(_controllerKey))
            {
                controllerName = (string)routeValues[_controllerKey];
            }

            return controllerName.StartsWith("Admin", StringComparison.OrdinalIgnoreCase);

        }

    }

}
