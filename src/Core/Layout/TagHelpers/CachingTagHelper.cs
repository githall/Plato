using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PlatoCore.Models.Shell;

namespace PlatoCore.Layout.TagHelpers
{

    /// <summary>
    /// Override the default cache tag helper to provide per-tenant caching.
    /// </summary>
    [HtmlTargetElement("caching")]
    public class CachingTagHelper : CacheTagHelper
    {

        public CachingTagHelper(         
            CacheTagHelperMemoryCacheFactory factory,
            IShellSettings shellSettings,
            HtmlEncoder htmlEncoder) : base(factory, htmlEncoder)
        {
            VaryBy += shellSettings.Name;
        }

    }

}
