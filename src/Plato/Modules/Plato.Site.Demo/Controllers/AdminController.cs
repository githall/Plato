using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Site.Demo.Models;
using Plato.Site.Demo.ViewModels;
using Plato.Internal.Layout;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Navigation;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Site.Demo.Services;
using Plato.Internal.Models.Users;
using Plato.Users.Services;

namespace Plato.Site.Demo.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {
        private readonly IViewProviderManager<DemoSettings> _viewProvider;
        private readonly IAuthorizationService _authorizationService;
        private readonly IPlatoUserManager<User> _platoUserManager;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IDemoService _demoService;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IViewProviderManager<DemoSettings> viewProvider,
            IAuthorizationService authorizationService,
             IPlatoUserManager<User> platoUserManager,
            IBreadCrumbManager breadCrumbManager,
            IDemoService demoService,
            IAlerter alerter)
        {
       
            _authorizationService = authorizationService;
            _breadCrumbManager = breadCrumbManager;
            _platoUserManager = platoUserManager;
            _viewProvider = viewProvider;
            _demoService = demoService;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }
        
        public async Task<IActionResult> Index()
        {

            // Ensure we have permission
            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditTwitterSettings))
            //{
            //    return Unauthorized();
            //}
            
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Settings"], channels => channels
                    .Action("Index", "Admin", "Plato.Settings")
                    .LocalNav()
                ).Add(S["Demo"]);
            });

            // Return view
            return View((LayoutViewModel)await _viewProvider.ProvideEditAsync(new DemoSettings(), this));

        }
        
        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(DemoSettingsViewModel viewModel)
        {

            // Ensure we have permission
            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditTwitterSettings))
            //{
            //    return Unauthorized();
            //}

            // Execute view providers ProvideUpdateAsync method
            await _viewProvider.ProvideUpdateAsync(new DemoSettings(), this);

            // Add alert
            _alerter.Success(T["Settings Updated Successfully!"]);

            // Reidrect to success
            return RedirectToAction(nameof(Index));
            
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        public async Task<IActionResult> InstallEntities(DemoSettingsViewModel viewModel)
        {

            var result = await _demoService.InstallEntitiesAsync("Plato.Discuss");
            if (result.Succeeded)
            {

                // Add alert
                _alerter.Success(T["Sample Data Added Successfully!"]);

                // Redirect to success
                return RedirectToAction(nameof(Index));

            }

            // If we reach this point something went wrong
            foreach (var error in result.Errors)
            {
                // Add errors
                _alerter.Danger(T[error.Description]);

            }

            // And redirect to display
            return RedirectToAction(nameof(Index));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        public async Task<IActionResult> InstallUsers(DemoSettingsViewModel viewModel)
        {

            var result = await _demoService.InstallUsersAsync();
            if (result.Succeeded)
            {

                // Add alert
                _alerter.Success(T["Sample Data Added Successfully!"]);

                // Redirect to success
                return RedirectToAction(nameof(Index));

            }

            // If we reach this point something went wrong
            foreach (var error in result.Errors)
            {
                // Add errors
                _alerter.Danger(T[error.Description]);

            }

            // And redirect to display
            return RedirectToAction(nameof(Index));

        }

    }

}
