using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
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

            // Get adapters for our identifier
            var matchingAdapters = adapters.First(this.Id);

            // No adapters for identifier
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

    }

}
