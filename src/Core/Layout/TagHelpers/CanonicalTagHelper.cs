using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PlatoCore.Hosting.Web.Abstractions;

namespace PlatoCore.Layout.TagHelpers
{

    [HtmlTargetElement("canonical", Attributes = "href", ParentTag = "head")]
    public class CanonicalTagHelper : TagHelper
    {

        private const string Http = "http://";
        private const string Https = "https://";
        private const string RelValue = "canonical";

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("href")]
        public string Href { get; set; }
                
        private readonly IContextFacade _contextFacade;

        public CanonicalTagHelper(IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            if (!string.IsNullOrEmpty(Href))
            {
                var isHttp = Href.IndexOf(Http, StringComparison.OrdinalIgnoreCase) >= 0;
                var isHttps = Href.IndexOf(Https, StringComparison.OrdinalIgnoreCase) >= 0;
                output.TagName = "link";
                output.Attributes.SetAttribute("rel", RelValue);
                output.Attributes.SetAttribute("href", isHttp || isHttps
                    ? Href
                    : await _contextFacade.GetBaseUrlAsync() + Href);               
            }
            else
            {
                output.SuppressOutput();       
            }

        }

    }

}
