using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Layout.TagHelperAdapters.Abstractions;

namespace PlatoCore.Layout.TagHelpers
{

    [HtmlTargetElement(Attributes = "asp-id")]
    public class TagHelperAdapterTagHelper : TagHelper
    {

        [HtmlAttributeName("asp-id")]
        public string Id { get; set; }

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            // Populated via TagHelperAdapterModelFilter
            var adapters = ViewContext.HttpContext.Items[typeof(TagHelperAdapterCollection)] as ITagHelperAdapterCollection;

            if (adapters == null)
            {
                return;
            }

            if (adapters.Adapters == null)
            {
                return;
            }

            if (adapters.Adapters.Count == 0)
            {
                return;
            }

            // Get executing views name
            var viewName = GetViewName(ViewContext.ExecutingFilePath);

            // We need a name
            if (string.IsNullOrEmpty(viewName))
            {
                return;
            }

            // Get adapters for our view and tag identifier
            var matchingAdapters = adapters.First(viewName, Id);

            // No adapters found
            if (matchingAdapters == null)
            {
                return;
            }

            // Process adapters
            foreach (var adapter in matchingAdapters)
            {
                await adapter.ProcessAsync(context, output);
            }

        }

        string GetViewName(string executingFilePath)
        {

            if (string.IsNullOrEmpty(executingFilePath))
            {
                return string.Empty;
            }

            var parts = executingFilePath.Split('/');

            if (parts.Length == 0)
            {
                return string.Empty;
            }

            var isComponent = parts.Contains("Components") && parts.Contains("Default.cshtml");

            if (isComponent)
            {
                return parts[parts.Length - 2];
            }

            return string.Empty;

        }

    }

}
