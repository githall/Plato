using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PlatoCore.Layout.TagHelpers
{
    [HtmlTargetElement("card-title", ParentTag = "card")]
    public class CardTitleTagHelper : TagHelper
    {

        public string Class { get; set; } = "card-header";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            var childContent = await output.GetChildContentAsync();
            var modalContext = (CardContext)context.Items[typeof(CardContext)];
            modalContext.Title = new CardSection()
            {
                CssClass = Class,
                Content = childContent
            };
            output.SuppressOutput();
        }

    }

}
