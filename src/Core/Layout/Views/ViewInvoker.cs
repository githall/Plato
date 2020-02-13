using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout.Views
{

    public class ViewInvoker : IViewInvoker
    {

        public ViewContext ViewContext { get; set; }
        
        private readonly IPlatoViewComponentInvoker _platoViewComponentInvoker;
        private readonly IPlatoPartialViewInvoker _platoPartialViewInvoker;
        private readonly ILogger<ViewInvoker> _logger;        

        public ViewInvoker(
            IPlatoViewComponentInvoker platoViewComponentInvoker,
            IPlatoPartialViewInvoker platoPartialViewInvoker,
            ILogger<ViewInvoker> logger)
        {
            _platoViewComponentInvoker = platoViewComponentInvoker;
            _platoPartialViewInvoker = platoPartialViewInvoker;            
            _logger = logger;
        }

        // Implementation

        public void Contextualize(ViewContext viewContext)
        {
            ViewContext = viewContext;
        }

        public async Task<IHtmlContent> InvokeAsync(IView view)
        {

            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (ViewContext == null)
            {
                throw new Exception(
                    "The ViewContext must be set via the Contextualize method before calling the InvokeAsync method");
            }        

            // Compiled views are invoked directly
            if (view as CompiledView != null)
            {
                return await InvokeCompiledViewAsync((ICompiledView)view);
            }

            // View components use an anonymous type for the parameters argument
            // this anonymous type is emitted as an actual type by the compiler but
            // marked with the CompilerGeneratedAttribute. If we find this attribute
            // on the model we'll treat this view as a ViewComponent and invoke accordingly
            if (IsComponent(view.Model))
            {
                return await InvokeViewComponentAsync(view);
            }

            // Else we have a partial view
            return await InvokePartialViewAsync(view);

        }

        // ------------------

        async Task<IHtmlContent> InvokeCompiledViewAsync(ICompiledView view)
        {
            return await view.InvokeAsync(ViewContext);
        }

        async Task<IHtmlContent> InvokePartialViewAsync(IView view)
        {
            _platoPartialViewInvoker.Contextualize(ViewContext);
            return await _platoPartialViewInvoker.InvokeAsync(view);
        }

        async Task<IHtmlContent> InvokeViewComponentAsync(IView view)
        {
            _platoViewComponentInvoker.Contextualize(ViewContext);
            return await _platoViewComponentInvoker.InvokeAsync(view);
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
