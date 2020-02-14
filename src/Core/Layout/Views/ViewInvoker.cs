using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout.Views
{

    public class ViewInvoker : IViewInvoker
    {

        public ViewContext ViewContext { get; set; }

        private readonly IPlatoViewComponentInvoker _viewComponentInvoker;
        private readonly IPlatoPartialViewInvoker _partialViewInvoker;   

        public ViewInvoker(
            IPlatoViewComponentInvoker viewComponentInvoker,
            IPlatoPartialViewInvoker partialViewInvoker)
        {
            _viewComponentInvoker = viewComponentInvoker;
            _partialViewInvoker = partialViewInvoker;                  
        }

        // Implementation

        public IViewInvoker Contextualize(ViewContext viewContext)
        {
            ViewContext = viewContext;
            return this;
        }

        public async Task<IHtmlContent> InvokeAsync(IView view)
        {

            Validate(view);

            // Compiled view
            if (view is ICompiledView)
            {
                return await ((ICompiledView)view)
                    .Contextualize(ViewContext)
                    .InvokeAsync();
                }

            // View comoponent
            if (IsComponent(view.Model))
            {
                return await _viewComponentInvoker
                    .Contextualize(ViewContext)
                    .InvokeAsync(view);
            }

            // Partial view      
            return await _partialViewInvoker
                .Contextualize(ViewContext)
                .InvokeAsync(view);

        }

        // ------------------

        void Validate(IView view)
        {

            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (ViewContext == null)
            {
                throw new Exception("The ViewContext must be set via the Contextualize method before calling the InvokeAsync method");
            }

        }

        bool IsComponent(object model)
        {

            // We need a model to inspect
            if (model == null)
            {
                return false;
            }

            return model.GetType().IsAnonymousType();

        }

    }

}
