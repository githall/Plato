using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PlatoCore.Layout.TagHelperAdapters.Abstractions;

namespace PlatoCore.Layout.TagHelpers
{

    [HtmlTargetElement(Attributes = "asp-id")]
    public class TagHelperAdapterTagHelper : TagHelper
    {

        [HtmlAttributeName("asp-id")]
        public string Id { get; set; }

        [HtmlAttributeName("asp-adapters")]
        public ITagHelperAdapterCollection AdapterCollection { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            // No alterations
            if (AdapterCollection == null)
            {
                return;
            }

            // No alterations
            if (AdapterCollection.Adapters.Count <= 0)
            {
                return;
            }

            // Get adapters for our identifier
            var adapters = AdapterCollection.First(this.Id);

            // No adapters for identifier
            if (adapters == null)
            {
                return;
            }

            // Process adapters
            foreach (var adapter in adapters)
            {
                await adapter.ProcessAsync(context, output);
            }

        }

    }

}
