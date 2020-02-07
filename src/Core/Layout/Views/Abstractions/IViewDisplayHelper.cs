using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace PlatoCore.Layout.Views.Abstractions
{
    public interface IViewDisplayHelper
    {
        Task<IHtmlContent> DisplayAsync(IView view);
    }
}
