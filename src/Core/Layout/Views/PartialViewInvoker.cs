using System;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout.Views
{

    public class PartialViewInvoker : IPlatoPartialViewInvoker
    {
   
        private readonly ILogger<PartialViewInvoker> _logger;
        private readonly ICompositeViewEngine _viewEngine;

        public PartialViewInvoker(
            ILogger<PartialViewInvoker> logger,
            ICompositeViewEngine viewEngine) 
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _viewEngine = viewEngine ?? throw new ArgumentNullException(nameof(viewEngine));            
        }

        public ViewContext ViewContext { get; set; }

        public void Contextualize(ViewContext viewContext)
        {
            ViewContext = viewContext;
        }

        public async Task<IHtmlContent> InvokeAsync(Abstractions.IView view)
        {

            // We always need a view name to invoke
            if (string.IsNullOrEmpty(view.ViewName))
            {
                throw new ArgumentNullException(nameof(view.ViewName));
            }
          
            var result = FindView(view.ViewName);
            if (!result.Success)
            {
                throw new Exception($"A view with the name \"{view.ViewName}\" could not be found!");
            }

            var builder = new HtmlContentBuilder();
            using (var writer = new StringWriter())
            {
                // Render view
                await RenderPartialViewAsync(writer, view.Model, ViewContext.ViewData, result.View);
                // Write results
                builder.WriteTo(writer, HtmlEncoder.Default);
                // Return builder
                return builder.SetHtmlContent(writer.ToString());
            }

        }

        // -----------

        ViewEngineResult FindView(string partialName)
        {

            var viewEngineResult = _viewEngine.GetView(ViewContext.ExecutingFilePath, partialName, isMainPage: false);
            var getViewLocations = viewEngineResult.SearchedLocations;
            if (!viewEngineResult.Success)
            {
                viewEngineResult = _viewEngine.FindView(ViewContext, partialName, isMainPage: false);
            }

            if (!viewEngineResult.Success)
            {
                var searchedLocations = Enumerable.Concat(getViewLocations, viewEngineResult.SearchedLocations);
                return ViewEngineResult.NotFound(partialName, searchedLocations);
            }

            return viewEngineResult;
        }

        async Task RenderPartialViewAsync(
            TextWriter writer,
            object model,
            ViewDataDictionary viewData,
            Microsoft.AspNetCore.Mvc.ViewEngines.IView view)
        {     

            var viewContext = new ViewContext(
                ViewContext, 
                view, 
                new ViewDataDictionary<object>(viewData ?? ViewContext.ViewData, model), 
                writer);

            using (view as IDisposable)
            {
                await view.RenderAsync(viewContext);             
            }

        }

    }

}
