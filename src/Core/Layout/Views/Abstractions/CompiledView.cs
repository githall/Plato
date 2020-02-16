using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PlatoCore.Layout.Views.Abstractions
{
    public abstract class CompiledView : LayoutZoneView, ICompiledView
    {

        public ViewContext ViewContext { get; set; }

        public ICompiledView Contextualize(ViewContext viewContext)
        {
            ViewContext = viewContext;
            return this;
        }

        public abstract Task<IHtmlContent> InvokeAsync();

    }

}
