using System;
using PlatoCore.Layout.TagHelperAdapters.Abstractions;
using PlatoCore.Layout.ViewFilters.Abstractions;

namespace PlatoCore.Layout.ViewFilters
{
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

                // If successful register our tag helper adapters on the current context
                // This allows the TagHelperAdapterTagHelper to access our adapters
                // from the context and ensures we don't need to pass in our tag helper 
                // adapters via an attribute on our TagHelperAdapterTagHelper 
                // The goal is to ensure the TagHelperAdapterTagHelper requires minimal configuration
                if (model != null)
                {
                    context.ViewContext.HttpContext.Items[typeof(TagHelperAdapterCollection)] = model.TagHelperAdapters;
                }
                
            }
        }

        public void OnViewComponentExecuted(ViewComponentExecutedContext context)
        {
        }

    }

}
