using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PlatoCore.Layout.Views.Abstractions
{
    public abstract class CompiledView : PositionedView, ICompiledView
    {

        public abstract Task<IHtmlContent> InvokeAsync(ViewContext context);

    }

}
