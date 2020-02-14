using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout.Views
{

    public class ViewFactory : IViewFactory
    {

        private readonly IViewDescriptorCollection _viewDescriptors;
        private readonly IViewInvoker _viewInvoker;

        public ViewFactory(
            IViewDescriptorCollection viewDescriptors,
            IViewInvoker viewInvoker)
        {
            _viewDescriptors = viewDescriptors;
            _viewInvoker = viewInvoker;
        }

        public async Task<IHtmlContent> InvokeAsync(ViewDisplayContext displayContext)
        {

            // Apply view & model alterations
            if (displayContext.ViewAdapterResults != null)
            {
                foreach (var viewAdapterResult in displayContext.ViewAdapterResults)
                {

                    // Apply view alterations
                    var viewAlterations = viewAdapterResult.ViewAlterations;
                    if (viewAlterations.Count > 0)
                    {
                        foreach (var alteration in viewAlterations)
                        {
                            displayContext.ViewDescriptor.View.ViewName = alteration;
                        }
                    }

                    // Apply model alterations
                    var modelAlterations = viewAdapterResult.ModelAlterations;
                    if (modelAlterations.Count > 0)
                    {
                        foreach (var alteration in modelAlterations)
                        {
                            var model = await alteration(displayContext.ViewDescriptor.View.Model);
                            if (model != null)
                            {
                                displayContext.ViewDescriptor.View.Model = model;
                            }                         
                        }
                    }

                }

            }

            // Add descriptor
            var descriptor = _viewDescriptors.Add(displayContext.ViewDescriptor);

            // Invoke view             
            var htmlContent = await _viewInvoker
                .Contextualize(displayContext.ViewContext)
                .InvokeAsync(descriptor.View);

            // Apply output alterations
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
