using Microsoft.AspNetCore.Mvc.Razor;

namespace PlatoCore.Layout.LocationExpander
{
    public interface IViewLocationExpanderProvider : IViewLocationExpander
    {
        int Priority { get; }
    }
}
