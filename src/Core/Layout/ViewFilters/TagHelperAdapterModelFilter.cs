using PlatoCore.Layout.TagHelperAdapters.Abstractions;
using PlatoCore.Layout.ViewFilters.Abstractions;

namespace PlatoCore.Layout.ViewFilters
{

    /// <summary>
    /// This view filter adds tag helper adapters to the current context before a view component executes
    /// This only occurrs if the view components view model implements the ITagHelperAdapterAwareViewModel interface
    /// This allows the TagHelperAdapterTagHelper to access our adapters from the context and ensures we don't 
    /// need to pass in our adapters via a mark-up attribute on our TagHelperAdapterTagHelper 
    /// The goal is to ensure the TagHelperAdapterTagHelper requires minimal configuration (only an id tp act upon)
    /// </summary>
    public class TagHelperAdapterModelFilter : IViewComponentFilter
    {

        public void OnViewComponentExecuting(ViewComponentExecutingContext context)
        {

            if (context.ViewData == null)
            {
                return;
            }

            if (context.ViewData.Model == null)
            {
                return;
            }

            // Does the view components view model implement the ITagHelperAdapterAwareViewModel interface
            if (typeof(ITagHelperAdapterAwareViewModel).IsAssignableFrom(context.ViewData.Model.GetType()))
            {
                // Attempt to safely cast
                var model = context.ViewData.Model as ITagHelperAdapterAwareViewModel;
                if (model != null)
                {
                    // Add adapters to the context before the view component executes
                    context.ViewContext.HttpContext.Items[typeof(TagHelperAdapterCollection)] = model.TagHelperAdapters;
                }
            }

        }

        public void OnViewComponentExecuted(ViewComponentExecutedContext context)
        {
        }

    }

}
