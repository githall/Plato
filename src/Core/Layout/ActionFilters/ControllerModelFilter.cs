using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PlatoCore.Layout.ActionFilters
{
    public class ControllerModelFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {

            // The controller action didn't return a view result so no need to continue execution
            var result = context.Result as ViewResult;
            if (result == null)
            {
                return;
            }

            if (result.Model == null)
            {
                return;
            }

            var controller = context.Controller as Controller;
            if (controller != null)
            {          
                controller.HttpContext.Items[result.Model.GetType()] = result.Model;
            }

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {      
        }

    }

}
