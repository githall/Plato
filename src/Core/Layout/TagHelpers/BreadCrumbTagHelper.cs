using System;
using System.Text;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PlatoCore.Navigation.Abstractions;

namespace PlatoCore.Layout.TagHelpers
{

    [HtmlTargetElement("breadcrumb")]
    public class BreadCrumbTagHelper : TagHelper
    {

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly HtmlEncoder _htmlEncoder;

        public BreadCrumbTagHelper(
            IActionContextAccessor actionContextAccessor,
            IBreadCrumbManager breadCrumbManager)
        {
            _actionContextAccessor = actionContextAccessor;
            _breadCrumbManager = breadCrumbManager;
            _htmlEncoder = HtmlEncoder.Default;
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            output.TagName = "ol";
            output.TagMode = TagMode.StartTagAndEndTag;
            
            var items = _breadCrumbManager
                .BuildMenu(_actionContextAccessor.ActionContext);

            if (items != null)
            {
                output.PreContent.SetHtmlContent(BuildBreadCrumb(items));
            }

            return Task.CompletedTask;

        }

        string BuildBreadCrumb(IEnumerable<MenuItem> items)
        {

            var sb = new StringBuilder();
            foreach (var item in items)
            {
                BuildBreadCrumbItem(item, sb);
            }

            return sb.ToString();

        }

        void BuildBreadCrumbItem(MenuItem item, StringBuilder sb)
        {

            var hasUrl = !String.IsNullOrEmpty(item.Href);

            sb.Append("<li  class=\"breadcrumb-item\">");

            if (hasUrl)
            {
                sb.Append("<a href=\"").Append(item.Href).Append("\">");
            }
            
            // Ensure item.Text is always encoded
            // This can contain user supplied input
            sb.Append(_htmlEncoder.Encode(item.Text));

            if (hasUrl)
            {
                sb.Append("</a>");
            }

            sb.Append("</li>");

        }

    }

}
