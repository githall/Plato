using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Layout.ViewFilters.Abstractions;

namespace PlatoCore.Layout.Views.Abstractions
{

    public abstract class ViewComponentBase : ViewComponent
    {
        public IViewComponentResult View(object model)
        {

            var filters = HttpContext.RequestServices.GetServices<IViewComponentFilter>();

            var viewData = new ViewDataDictionary(ViewData)
            {
                Model = model
            };

            var viewName = ViewComponentContext?.ViewComponentDescriptor?.ShortName ?? string.Empty;

            foreach (var filter in filters)
            {
                filter.OnViewComponentExecuting(new ViewComponentExecutingContext()
                {
                    ViewName = viewName,
                    ViewData = viewData,
                    ViewContext = ViewContext
                });
            }

            var result = base.View(model);

            foreach (var filter in filters)
            {
                filter.OnViewComponentExecuted(new ViewComponentExecutedContext()
                {
                    ViewName = result.ViewName,
                    ViewData = result.ViewData,
                    ViewContext = ViewContext
                });
            }

            return result;

        }

    }

}
