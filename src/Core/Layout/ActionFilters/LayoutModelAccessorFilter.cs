using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace PlatoCore.Layout.ActionFilters
{

    public class LayoutModelAccessorFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // The controller action didn't return a view result so no need to continue execution
            var result = context.Result as ViewResult;

            // Check early to ensure we are working with a LayoutViewModel
            var model = result?.Model as LayoutViewModel;
            if (model == null)
            {
                return;
            }

            // Update accessor
            var layoutModelAccessor = context.HttpContext.RequestServices.GetRequiredService<ILayoutModelAccessor>();
            layoutModelAccessor.LayoutViewModel = model;

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
       
         
        }

    }

}
