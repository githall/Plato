using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Site.Demo.Controllers
{

    public class HomeController : Controller, IUpdateModel
    {
     
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public HomeController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,           
            IAlerter alerter)
        {       
     
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }
            
        [HttpPost, ValidateAntiForgeryToken]
        public Task<IActionResult> Login(string returnUrl)
        {        

            // Add alert
            _alerter.Success(T["Admin Login Successful!"]);

            // Redirect to return url
            return Task.FromResult(RedirectToLocal(returnUrl));

        }
        
        IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("~/");
            }
        }


    }

}
