using Microsoft.AspNetCore.Mvc.Rendering;

namespace PlatoCore.Layout.Views.Abstractions
{
    public interface IViewHelperFactory
    {
        IViewDisplayHelper CreateHelper(ViewContext context);
    }

}
