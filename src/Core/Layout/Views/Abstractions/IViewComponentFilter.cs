using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace PlatoCore.Layout.Views.Abstractions
{

    public class ViewComponentExecutingContext
    {

        public string ViewName { get; set; }

        public ViewDataDictionary ViewData { get; set; }

        public ViewContext ViewContext  { get; set; }

    }

    public class ViewComponentExecutedContext : ViewComponentExecutingContext
    {
    }

    public interface IViewComponentFilter
    {

        void OnViewComponentExecuting(ViewComponentExecutingContext context);

        void OnViewComponentExecuted(ViewComponentExecutedContext context);

    }

}
