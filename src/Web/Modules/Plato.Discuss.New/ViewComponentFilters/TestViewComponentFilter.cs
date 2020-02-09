using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Discuss.New.ViewComponentFilters
{

    public class TestViewComponentFilter : IViewComponentFilter
    {

        public void OnViewComponentExecuted(ViewComponentExecutedContext context)
        {

            var model = context.ViewData.Model;
            if (model == null)
            {
                return;
            }

            context.ViewContext.HttpContext.Items[model.GetType()] = model;

        }

        public void OnViewComponentExecuting(ViewComponentExecutingContext context)
        {   
        }

    }

}
