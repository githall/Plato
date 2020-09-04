using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using PlatoCore.Layout.ModelBinding;

namespace Plato.Spaces.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"
    
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController()
        {      
        }

        #endregion

        #region "Actions"

        // -----------------
        // Index
        // -----------------

        public IActionResult Index()
        {
            return View();
        }

        #endregion

    }

}
