using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using PlatoCore.Layout;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Navigation;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Tenants.ViewModels;
using PlatoCore.Models.Shell;

namespace Plato.Tenants.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {
        private readonly IViewProviderManager<ShellSettings> _viewProvider;
        private readonly IAuthorizationService _authorizationService;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IViewProviderManager<ShellSettings> viewProvider,
            IAuthorizationService authorizationService,
            IBreadCrumbManager breadCrumbManager,
            IAlerter alerter)
        {
       
            _authorizationService = authorizationService;
            _breadCrumbManager = breadCrumbManager;
            _viewProvider = viewProvider;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }
        
        public async Task<IActionResult> Index(TenantIndexOptions opts)
        {

            //// Ensure we have permission
            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditSlackSettings))
            //{
            //    return Unauthorized();
            //}

            if (opts == null)
            {
                opts = new TenantIndexOptions();
            }

            // Configure breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Settings"], channels => channels
                    .Action("Index", "Admin", "Plato.Settings")
                    .LocalNav()
                ).Add(S["Slack"]);
            });

            // Build view model
            var viewModel = new TenantIndexViewModel()
            {
                Options = opts               
            };

            // Add view model to context
            this.HttpContext.Items[typeof(TenantIndexViewModel)] = viewModel;

            // Return view
            return View((LayoutViewModel) await _viewProvider.ProvideIndexAsync(new ShellSettings(), this));
            
        }
        
        //[HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        //public async Task<IActionResult> IndexPost(SlackSettingsViewModel viewModel)
        //{

        //    // Ensure we have permission
        //    if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditSlackSettings))
        //    {
        //        return Unauthorized();
        //    }

        //    // Execute view providers ProvideUpdateAsync method
        //    await _viewProvider.ProvideUpdateAsync(new PlatoSlackSettings(), this);
        
        //    // Add alert
        //    _alerter.Success(T["Settings Updated Successfully!"]);
      
        //    return RedirectToAction(nameof(Index));
            
        //}
      
    }

}
