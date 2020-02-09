using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using PlatoCore.Layout.EmbeddedViews;

namespace PlatoCore.Layout.ViewHelpers
{

    public class HtmlViewHelper : EmbeddedView
    {

        private readonly string _html;

        public HtmlViewHelper(string html)
        {
            _html = html;
        }
        
        public override Task<IHtmlContent> InvokeAsync()
        {
            return Task.FromResult((IHtmlContent)new HtmlString(_html));
        }

    }

}
