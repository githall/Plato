using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PlatoCore.Layout.Models;

namespace PlatoCore.Layout.TagHelpers
{

    [HtmlTargetElement(Attributes = "asp-id")]
    public class TagAlterationTagHelper : TagHelper
    {

        [HtmlAttributeName("asp-id")]
        public string Id { get; set; }

        [HtmlAttributeName("asp-alterations")]
        public TagAlterations Alterations { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            // No alterations
            if (Alterations == null)
            {
                return;
            }

            // No alterations
            if (Alterations.Count <= 0)
            {
                return;
            }

            // Get alterations for our identifier
            var alterations = Alterations.First(this.Id);

            // No alterations for identifier
            if (alterations == null)
            {
                return;
            }

            // Process alterations
            foreach (var alteration in alterations)
            {
                await alteration.ProcessAsync(context, output);
            }

        }

    }

}
