using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.GitHub.Models;
using Plato.GitHub.ViewModels;
using PlatoCore.Layout;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Navigation;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.GitHub.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {
        
        private readonly IAuthorizationService _authorizationService;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IViewProviderManager<PlatoGitHubSettings> _viewProvider;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IViewProviderManager<PlatoGitHubSettings> viewProvider,
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
        
        public async Task<IActionResult> Index()
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditGitHubSettings))
            {
                return Unauthorized();
            }
            
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Settings"], channels => channels
                    .Action("Index", "Admin", "Plato.Settings")
                    .LocalNav()
                ).Add(S["GitHub"]);
            });

            // Return view
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(new PlatoGitHubSettings(), this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(GoogleSettingsViewModel viewModel)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditGitHubSettings))
            {
                return Unauthorized();
            }

            // Execute view providers ProvideUpdateAsync method
            await _viewProvider.ProvideUpdateAsync(new PlatoGitHubSettings(), this);
        
            // Add alert
            _alerter.Success(T["Settings Updated Successfully!"]);
      
            return RedirectToAction(nameof(Index));
            
        }
      
    }

}
