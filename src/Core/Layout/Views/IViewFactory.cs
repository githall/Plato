using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace PlatoCore.Layout.Views
{
    public interface IViewFactory
    {

        Task<IHtmlContent> InvokeAsync(ViewDisplayContext displayContext);

    }

}
