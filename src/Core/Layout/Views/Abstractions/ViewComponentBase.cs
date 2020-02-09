using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace PlatoCore.Layout.Views.Abstractions
{

    public abstract class ViewComponentBase : ViewComponent
    {
        public IViewComponentResult View(object model)
        {

            var filters = HttpContext.RequestServices.GetServices<IViewComponentFilter>();

            foreach (var filter in filters)
            {
                filter.OnViewComponentExecuting(new ViewComponentExecutingContext()
                {
                    ViewName = null,
                    ViewData = ViewData,
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
