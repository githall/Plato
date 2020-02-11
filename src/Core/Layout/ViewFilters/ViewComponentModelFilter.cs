using PlatoCore.Layout.ViewFilters.Abstractions;

namespace PlatoCore.Layout.ViewFilters
{

    /// <summary>
    /// This view component filter stores models returned via view components within the current HttpContext.Items dictionary 
    /// This ensures the model is available throughout the request, for example within child view components. 
    /// </summary>
    public class ViewComponentModelFilter : IViewComponentFilter
    {

        public void OnViewComponentExecuting(ViewComponentExecutingContext context)
        {
        }

        public void OnViewComponentExecuted(ViewComponentExecutedContext context)
        {

            if (context.ViewData == null)
            {
                return;
            }

            if (context.ViewData.Model == null)
            {
                return;
            }

            context.ViewContext.HttpContext.Items[context.ViewData.Model.GetType()] = context.ViewData.Model;

        }

    }

}
