using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Models.Users;
using Plato.Site.Demo.Models;
using System;

namespace Plato.Site.Demo.Controllers
{

    public class HomeController : Controller, IUpdateModel
    {

        private readonly SignInManager<User> _signInManager;
        private readonly IContextFacade _contextFacade;
        private readonly IAlerter _alerter;

        private readonly DemoOptions _demoOptions;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public HomeController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            SignInManager<User> signInManager,
            IOptions<DemoOptions> demoOptions,
            IContextFacade contextFacade,
            IAlerter alerter)
        {

            _demoOptions = demoOptions.Value;
            _contextFacade = contextFacade;
            _signInManager = signInManager;
            _alerter = alerter;
            
            T = htmlLocalizer;
            S = stringLocalizer;

        }

        [HttpPost]
        public async Task<IActionResult> Login(string returnUrl)
        {

            if (string.IsNullOrEmpty(_demoOptions.AdminUserName))
            {
                throw new ArgumentNullException(nameof(_demoOptions.AdminUserName));
            }

            if (string.IsNullOrEmpty(_demoOptions.AdminPassword))
            {
                throw new ArgumentNullException(nameof(_demoOptions.AdminPassword));
            }

            // Get sign in result
            var result = await _signInManager.PasswordSignInAsync(
                _demoOptions.AdminUserName,
                _demoOptions.AdminPassword,
                isPersistent: true,
                lockoutOnFailure: false);

            // Success
            if (result.Succeeded)
            {
                // Add alert
                _alerter.Success(T["Admin Login Successful!"]);

                // Redirect to Plato.Admin
                return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                {
                    ["area"] = "Plato.Admin",
                    ["controller"] = "Admin",
                    ["action"] = "Index"
                }));

            }

            // Redirect to return url
            return RedirectToLocal(returnUrl);

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
