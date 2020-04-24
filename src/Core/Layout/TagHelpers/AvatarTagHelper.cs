using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Models.Users;

namespace PlatoCore.Layout.TagHelpers
{

    [HtmlTargetElement("avatar")]
    public class AvatarTagHelper : TagHelper
    {

        [HtmlAttributeName("avatar")]
        public UserAvatar Avatar { get; set; }

        private readonly IUrlHelper _urlHelper;

        public AvatarTagHelper(                      
            IActionContextAccessor actionContextAccesor,
            IUrlHelperFactory urlHelperFactory)
        {
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccesor.ActionContext);
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("role", "avatar");

            var img = new TagBuilder("span");

            if (Avatar != null)
            {

                if (!string.IsNullOrEmpty(Avatar.Url))
                {
                    img.Attributes.Add("style", $"background-image: url('{Avatar.Url}');");
                }
                else
                {

                    var url = _urlHelper.RouteUrl(new UrlRouteContext
                    {
                        Values = Avatar.DefaultRoute
                    });

                    img.Attributes.Add("style", $"background-image: url('{url}');");

                }

            }
            else
            {
                img.Attributes.Add("style", $"background-image: url('/images/photo.png');");
            }

            output.Content.SetHtmlContent(img.ToHtmlString());
            return Task.CompletedTask;

        }

    }

}
