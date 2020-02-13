using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Logging;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Layout.EmbeddedViews;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout.Views
{

    public class ViewInvoker : IViewInvoker
    {

        public ViewContext ViewContext { get; set; }
        
        private readonly IViewComponentHelper _viewComponentHelper;
        private readonly IPartialViewInvoker _partialViewInvoker;
        private readonly ILogger<ViewInvoker> _logger;        

        public ViewInvoker(            
            IViewComponentHelper viewComponentHelper,
            IPartialViewInvoker partialViewInvoker,
            ILogger<ViewInvoker> logger)
        {            
            _viewComponentHelper = viewComponentHelper;
            _partialViewInvoker = partialViewInvoker;            
            _logger = logger;
        }

        // Implementation

        public void Contextualize(ViewDisplayContext context)
        {
            ViewContext = context.ViewContext;
        }

        public async Task<IHtmlContent> InvokeAsync(IView view)
        {

            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (string.IsNullOrEmpty(view.ViewName))
            {
                throw new ArgumentNullException(nameof(view.ViewName));
            }

            // ** Hot code path ** - please modify carefully

            if (ViewContext == null)
            {
                throw new Exception(
                    "ViewContext must be set via the Contextualize method before calling the InvokeAsync method");
            }

            // Embedded views simply return the output generated within the view
            // It's the embedded views responsibility to perform model binding
            // Embedded views can leverage the current context within the InvokeAsync method
            if (view.EmbeddedView != null)
            {
                return await InvokeEmbeddedViewAsync(view.EmbeddedView);
            }

            // View components use an anonymous type for the parameters argument
            // this anonymous type is emitted as an actual type by the compiler but
            // marked with the CompilerGeneratedAttribute. If we find this attribute
            // on the model we'll treat this view as a ViewComponent and invoke accordingly
            if (IsComponent(view.Model))
            {
                return await InvokeViewComponentAsync(view.ViewName, view.Model);
            }

            // else we have a partial view
            return await InvokePartialViewAsync(view.ViewName, view.Model);

        }

        // ------------------

        async Task<IHtmlContent> InvokeEmbeddedViewAsync(IEmbeddedView view)
        {
            return await view.Contextualize(ViewContext).InvokeAsync();
        }

        async Task<IHtmlContent> InvokePartialViewAsync(string viewName, object model)
        {
            _partialViewInvoker.Contextualize(ViewContext);
            return await _partialViewInvoker.InvokeAsync(viewName, model, ViewContext.ViewData);
        }

        async Task<IHtmlContent> InvokeViewComponentAsync(string viewName, object arguments)
        {
            if (!(_viewComponentHelper is DefaultViewComponentHelper helper))
            {
                throw new ArgumentNullException(
                    $"{_viewComponentHelper.GetType()} cannot be converted to DefaultViewComponentHelper");
            }

            // Contextualize view component
            helper.Contextualize(ViewContext);

            // Log the invocation
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Attempting to invoke view component \"{viewName}\".");
            }

            try
            {
                return await _viewComponentHelper.InvokeAsync(viewName, arguments);
            }
            catch (Exception e)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(e,
                        $"An exception occurred whilst invoking the view component with name \"{viewName}\". {e.Message}");
                }
                throw;
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
