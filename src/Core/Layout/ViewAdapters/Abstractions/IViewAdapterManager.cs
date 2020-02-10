using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlatoCore.Layout.ViewAdapters.Abstractions
{

    public interface IViewAdapterManager
    {
        Task<IEnumerable<IViewAdapterResult>> GetViewAdaptersAsync(string name);
    }

}
