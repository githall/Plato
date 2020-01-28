using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlatoCore.Layout.ViewAdapters
{
    public interface IViewAdapterManager
    {
        Task<IEnumerable<IViewAdapterResult>> GetViewAdaptersAsync(string name);
    }

}
