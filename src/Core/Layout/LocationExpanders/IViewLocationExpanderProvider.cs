using Microsoft.AspNetCore.Mvc.Razor;

namespace PlatoCore.Layout.LocationExpanders
{
    public interface IViewLocationExpanderProvider : IViewLocationExpander
    {
        int Priority { get; }
    }
}
