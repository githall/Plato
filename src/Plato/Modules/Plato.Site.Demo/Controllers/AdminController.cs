using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Site.Demo.Models;
using Plato.Site.Demo.ViewModels;
using Plato.Internal.Layout;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Site.Demo.Services;

namespace Plato.Site.Demo.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly ISampleCategoriesService _sampleCategoriesService;
        private readonly IViewProviderManager<DemoSettings> _viewProvider;
        private readonly ISampleEntitiesService _sampleEntitiesService;
        private readonly ISampleLabelsService _sampleLabelsService;
        private readonly ISampleUsersService _sampleUsersService;        
        private readonly ISampleTagsService _sampleTagsService;
        private readonly IBreadCrumbManager _breadCrumbManager;

        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            ISampleCategoriesService sampleCategoriesService,
            IViewProviderManager<DemoSettings> viewProvider,
            ISampleEntitiesService sampleEntitiesService,            
            ISampleLabelsService sampleLabelsService,
            ISampleUsersService sampleUsersService,
            ISampleTagsService sampleTagsService,            
            IBreadCrumbManager breadCrumbManager,
            IAlerter alerter)
        {

            _sampleCategoriesService = sampleCategoriesService;
            _sampleEntitiesService = sampleEntitiesService;
            _sampleLabelsService = sampleLabelsService;            
            _sampleUsersService = sampleUsersService;
            _sampleTagsService = sampleTagsService;
            _breadCrumbManager = breadCrumbManager;
            _viewProvider = viewProvider;            
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }
        
        public async Task<IActionResult> Index()
        {

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

            // Execute view providers ProvideUpdateAsync method
            await _viewProvider.ProvideUpdateAsync(new DemoSettings(), this);

            // Add alert
            _alerter.Success(T["Settings Updated Successfully!"]);

            // Reidrect to success
            return RedirectToAction(nameof(Index));
            
        }

        // Entities

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> InstallEntities()
        {

            var result = await _sampleEntitiesService.InstallAsync();
            if (result.Succeeded)
            {

                // Add alert
                _alerter.Success(T["Entities Added Successfully!"]);

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

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UninstallEntities()
        {

            var result = await _sampleEntitiesService.UninstallAsync();
            if (result.Succeeded)
            {

                // Add alert
                _alerter.Success(T["Entities Removed Successfully!"]);

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

        // Users

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> InstallUsers()
        {

            var result = await _sampleUsersService.InstallAsync();
            if (result.Succeeded)
            {

                // Add alert
                _alerter.Success(T["Sample Users Added Successfully!"]);

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

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UninstallUsers()
        {

            var result = await _sampleUsersService.UninstallAsync();
            if (result.Succeeded)
            {

                // Add alert
                _alerter.Success(T["Sample Users Removed Successfully!"]);

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

        // Categories

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> InstallCategories()
        {

            var result = await _sampleCategoriesService.InstallAsync();
            if (result.Succeeded)
            {

                // Add alert
                _alerter.Success(T["Sample Categories Added Successfully!"]);

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

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UninstallCategories()
        {

            var result = await _sampleCategoriesService.UninstallAsync();
            if (result.Succeeded)
            {

                // Add alert
                _alerter.Success(T["Sample Categories Removed Successfully!"]);

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

        // Labels

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> InstallLabels()
        {

            var result = await _sampleLabelsService.InstallAsync();
            if (result.Succeeded)
            {

                // Add alert
                _alerter.Success(T["Sample Labels Added Successfully!"]);

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

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UninstallLabels()
        {

            var result = await _sampleLabelsService.UninstallAsync();
            if (result.Succeeded)
            {

                // Add alert
                _alerter.Success(T["Sample Labels Removed Successfully!"]);

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

        // Tags

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> InstallTags()
        {

            var result = await _sampleTagsService.InstallAsync();
            if (result.Succeeded)
            {

                // Add alert
                _alerter.Success(T["Sample Tags Added Successfully!"]);

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

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UninstallTags()
        {

            var result = await _sampleTagsService.UninstallAsync();
            if (result.Succeeded)
            {

                // Add alert
                _alerter.Success(T["Sample Tags Removed Successfully!"]);

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
