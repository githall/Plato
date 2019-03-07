﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Categories.Models;
using Plato.Articles.Categories.ViewModels;
using Plato.Categories.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Articles.Categories.ViewComponents
{

    public class ArticleCategoryListItemViewComponent : ViewComponent
    {
 
        public ArticleCategoryListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(CategoryHome categoryHome, CategoryIndexOptions categoryIndexOpts)
        {

            if (categoryIndexOpts == null)
            {
                categoryIndexOpts = new CategoryIndexOptions();
            }

            var model = new CategoryListItemViewModel<CategoryHome>()
            {
                Category = categoryHome,
                Options = categoryIndexOpts
            };

            return Task.FromResult((IViewComponentResult)View(model));

        }


    }


}