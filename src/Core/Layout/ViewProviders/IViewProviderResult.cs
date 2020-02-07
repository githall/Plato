using System.Collections.Generic;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout.ViewProviders
{

    public interface IViewProviderResult
    {
        IEnumerable<IView> Views { get; }
    }

}
