using System.Collections.Generic;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout.ViewProviders.Abstractions
{

    public interface IViewProviderResult
    {
        IEnumerable<IView> Views { get; }
    }

}
