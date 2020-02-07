using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace PlatoCore.Layout.Views.Abstractions
{
    public interface IViewFactory
    {

        Task<IHtmlContent> InvokeAsync(ViewDisplayContext displayContext);

    }

}
