using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PlatoCore.Layout.Views.Abstractions
{

    public class HtmlCompiledView : CompiledView
    {

        private readonly string _html;

        public HtmlCompiledView(string html)
        {
            _html = html;
        }
        
        public override Task<IHtmlContent> InvokeAsync(ViewContext context)
        {
            return Task.FromResult((IHtmlContent)new HtmlString(_html));
        }

    }

}
