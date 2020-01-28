using System.Collections.Generic;
using PlatoCore.Layout.Views;

namespace PlatoCore.Layout.ViewProviders
{

    public interface IViewProviderResult
    {
        IEnumerable<IView> Views { get; }
    }

}
