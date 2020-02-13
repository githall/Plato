using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Logging;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout.Views
{
    public class ViewComponentInvoker : IPlatoViewComponentInvoker
    {

        private readonly IViewComponentHelper _viewComponentHelper;
        private readonly ILogger<ViewComponentInvoker> _logger;

        public ViewContext ViewContext { get; set; }

        public ViewComponentInvoker(
            IViewComponentHelper viewComponentHelper,
            ILogger<ViewComponentInvoker> logger)
        {
            _viewComponentHelper = viewComponentHelper;
            _logger = logger;
        }

        public IViewInvoker Contextualize(ViewContext viewContext)
        {
            ViewContext = viewContext;
            return this;
        }

        public async Task<IHtmlContent> InvokeAsync(Abstractions.IView view)
        {

            // We always need a view name to invoke
            if (string.IsNullOrEmpty(view.ViewName))
            {
                throw new ArgumentNullException(nameof(view.ViewName));
            }

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
                _logger.LogInformation($"Attempting to invoke view component \"{view.ViewName}\".");
            }

            try
            {
                return await _viewComponentHelper.InvokeAsync(view.ViewName, view.Model);
            }
            catch (Exception e)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(e,
                        $"An exception occurred whilst invoking the view component with name \"{view.ViewName}\". {e.Message}");
                }
                throw;
            }

        }
    }
}
