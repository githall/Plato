using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace PlatoCore.Layout.Views.Abstractions
{

    public class HtmlCompiledView : CompiledView
    {

        private readonly string _html;

        public HtmlCompiledView(string html)
        {
            _html = html;
        }
        
        public override Task<IHtmlContent> InvokeAsync()
        {
            return Task.FromResult((IHtmlContent)new HtmlString(_html));
        }

    }

}
