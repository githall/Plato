using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Layout.Services;
using System.Threading.Tasks;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{

    [HtmlTargetElement("link", Attributes = "rel")]
    public class CanonicalUrlTagHelper : TagHelper
    {

        private const string RelValue = "canonical";

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("rel")]
        public string Rel { get; set; }
                
        private readonly ICanonicalUrlBuilder _canonicalUrlBuilder;
        private readonly IContextFacade _contextFacade;

        public CanonicalUrlTagHelper(
            ICanonicalUrlBuilder canonicalUrlBuilder,
            IContextFacade contextFacade)
        {
            _canonicalUrlBuilder = canonicalUrlBuilder;
            _contextFacade = contextFacade;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            if (Rel.Equals(RelValue, StringComparison.OrdinalIgnoreCase))
            {

                if (_canonicalUrlBuilder.RouteValues != null)
                {

                    var url = await _contextFacade.GetBaseUrlAsync() +
                        _contextFacade.GetRouteUrl(_canonicalUrlBuilder.RouteValues);
                    if (!string.IsNullOrEmpty(url))
                    {
                        output.Attributes.SetAttribute("rel", RelValue);
                        output.Attributes.SetAttribute("href", url);
                    }
                    else
                    {
                        output.SuppressOutput();

                    }

                }
                else
                {

                    if (!string.IsNullOrEmpty(_canonicalUrlBuilder.Url))
                    {
                        output.Attributes.SetAttribute("rel", RelValue);
                        output.Attributes.SetAttribute("href", _canonicalUrlBuilder.Url);
                    }
                    else
                    {
                        output.SuppressOutput();

                    }
                }

            }       

        }

    }

}
