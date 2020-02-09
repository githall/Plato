using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout.Views
{
    
    public class ViewFactory : IViewFactory
    {

        private readonly IViewInvoker _viewInvoker;
        private readonly IViewTable _viewTableManager;

        public ViewFactory(
            IViewTable viewTableManager,
            IViewInvoker viewInvoker)
        {
            _viewTableManager = viewTableManager;
            _viewInvoker = viewInvoker;
        }

        public async Task<IHtmlContent> InvokeAsync(ViewDisplayContext displayContext)
        {

            // Contextualize view invoker
            _viewInvoker.Contextualize(displayContext);

            // Apply view & model alterations
            if (displayContext.ViewAdapterResults != null)
            {
                foreach (var viewAdapterResult in displayContext.ViewAdapterResults)
                {

                    var updatedView = displayContext.ViewDescriptor.View;

                    // Apply view alterations
                    var viewAlterations = viewAdapterResult.ViewAlterations;
                    if (viewAlterations.Count > 0)
                    {
                        foreach (var alteration in viewAlterations)
                        {
                            updatedView.ViewName = alteration;
                        }
                    }

                    // Apply model alterations
                    var modelAlterations = viewAdapterResult.ModelAlterations;
                    if (modelAlterations.Count > 0)
                    {
                        foreach (var alteration in modelAlterations)
                        {
                            var model = await alteration?.Invoke(updatedView.Model);
                            if (model != null)
                            {
                                updatedView.Model = model;
                            }                         
                        }
                    }

                    displayContext.ViewDescriptor.View = updatedView;

                }

            }

            // Add descriptor
            var descriptor = _viewTableManager.Add(displayContext.ViewDescriptor);

            // Invoke view
            var htmlContent = await _viewInvoker.InvokeAsync(descriptor.View);
         
            // Apply adapter output alterations
            if (displayContext.ViewAdapterResults != null)
            {
                foreach (var viewAdapterResult in displayContext.ViewAdapterResults)
                {
                    var alterations = viewAdapterResult.OutputAlterations;
                    if (alterations.Count > 0)
                    {
                        foreach (var alteration in alterations)
                        {
                            htmlContent = alteration(htmlContent);
                        }
                    }

                }
            }

            return htmlContent;

        }

    }

}
