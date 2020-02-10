using PlatoCore.Layout.Views.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace PlatoCore.Layout.ViewComponentFilters
{

    /// <summary>
    /// This filter adds view component models to our globally accessible model collection.
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

            var modelCollection = context.ViewContext.HttpContext.RequestServices.GetService<IModelCollection>();
            modelCollection.AddOrUpdate(context.ViewData.Model);

            context.ViewContext.HttpContext.Items[context.ViewData.Model.GetType()] = context.ViewData.Model;

        }

    }

}
