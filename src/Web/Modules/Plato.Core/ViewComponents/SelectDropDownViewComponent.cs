﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Core.ViewComponents
{
  
    public class SelectDropDownViewComponent : ViewComponent
    {
  
        public SelectDropDownViewComponent(IContextFacade contextFacade)
        {
        }

        public Task<IViewComponentResult> InvokeAsync(SelectDropDownViewModel model)
        {

            if (model == null)
            {
                model = new SelectDropDownViewModel();
            }

            return Task.FromResult((IViewComponentResult) View(model));

        }

    }

}
