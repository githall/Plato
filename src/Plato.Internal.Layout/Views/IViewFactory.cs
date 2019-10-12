using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.Views
{
    public interface IViewFactory
    {

        Task<IHtmlContent> InvokeAsync(ViewDisplayContext displayContext);

    }

}
