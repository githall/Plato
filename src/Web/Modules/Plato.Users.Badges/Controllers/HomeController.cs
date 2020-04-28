using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Layout;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Badges;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Users.Badges.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IViewProviderManager<Badge> _badgeViewProvider;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IContextFacade _contextFacade;
      
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public HomeController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IViewProviderManager<Badge> badgeViewProvider,
            IBreadCrumbManager breadCrumbManager,
            IContextFacade contextFacade)
        {
            
            _badgeViewProvider = badgeViewProvider;
            _breadCrumbManager = breadCrumbManager;
            _contextFacade = contextFacade;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"

        public async Task<IActionResult> Index()
        {

            // Return Url for authentication purposes
            ViewData["ReturnUrl"] = _contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Users.Badges",
                ["controller"] = "Home",
                ["action"] = "Index"
            });

            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Badges"]);
            });
            
            // Return view
            return View((LayoutViewModel) await _badgeViewProvider.ProvideIndexAsync(new Badge(), this));

        }
        
        #endregion

    }

}
