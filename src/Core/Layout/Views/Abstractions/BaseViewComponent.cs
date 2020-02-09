using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace PlatoCore.Layout.Views.Abstractions
{

    public abstract class BaseController : Controller
    {
        public override ViewResult View(object model)
        {
            var result = base.View(model);
            var viewResultTable = HttpContext.RequestServices.GetService<IViewResultTable>();
            viewResultTable.Add(result);
            return result;
        }
    }

    public abstract class BaseViewComponent : ViewComponent
    {
        public IViewComponentResult View(object model)
        {
            var result = base.View(model);        
            var viewResultTable = HttpContext.RequestServices.GetService<IViewResultTable>();
            viewResultTable.Add(result);
            return result;
        }
    }

}
