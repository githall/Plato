using System.Threading.Tasks;

namespace PlatoCore.Layout.ViewAdapters.Abstractions
{

    public interface IViewAdapterProvider
    {

        string ViewName { get; set; }

        Task<IViewAdapterResult> ConfigureAsync(string viewName);

    }

}
