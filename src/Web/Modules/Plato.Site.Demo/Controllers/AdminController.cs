using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Site.Demo.Models;
using Plato.Site.Demo.ViewModels;
using PlatoCore.Layout;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Navigation.Abstractions;
using Plato.Site.Demo.Services;

namespace Plato.Site.Demo.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly ISampleEntityCategoriesService _sampleEntityCategoriesService;
        private readonly ISampleEntityLabelsService _sampleEntityLabelsService;
        private readonly ISampleEntityTagsService _sampleEntityTagsService;
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
            ISampleEntityCategoriesService sampleEntityCategoriesService,
            ISampleEntityLabelsService sampleEntityLabelsService,
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            ISampleCategoriesService sampleCategoriesService,
            IViewProviderManager<DemoSettings> viewProvider,
            ISampleEntityTagsService sampleEntityTagsService,
            ISampleEntitiesService sampleEntitiesService,            
            ISampleLabelsService sampleLabelsService,
            ISampleUsersService sampleUsersService,
            ISampleTagsService sampleTagsService,            
            IBreadCrumbManager breadCrumbManager,
            IAlerter alerter)
        {
            _sampleEntityCategoriesService = sampleEntityCategoriesService;
            _sampleEntityLabelsService = sampleEntityLabelsService;
            _sampleEntityTagsService = sampleEntityTagsService;
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

        // Entity Labels

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> InstallEntityLabels()
        {

            var result = await _sampleEntityLabelsService.InstallAsync();
            if (result.Succeeded)
            {

                // Add alert
                _alerter.Success(T["Sample Entity Labels Added Successfully!"]);

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
        public async Task<IActionResult> UninstallEntityLabels()
        {

            var result = await _sampleEntityLabelsService.UninstallAsync();
            if (result.Succeeded)
            {

                // Add alert
                _alerter.Success(T["Sample Entity Labels Removed Successfully!"]);

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

        // Entity Tags

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> InstallEntityTags()
        {

            var result = await _sampleEntityTagsService.InstallAsync();
            if (result.Succeeded)
            {

                // Add alert
                _alerter.Success(T["Sample Entity Tags Added Successfully!"]);

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
        public async Task<IActionResult> UninstallEntityTags()
        {

            var result = await _sampleEntityTagsService.UninstallAsync();
            if (result.Succeeded)
            {

                // Add alert
                _alerter.Success(T["Sample Entity Tags Removed Successfully!"]);

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

        // Entity Categories

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> InstallEntityCategories()
        {

            var result = await _sampleEntityCategoriesService.InstallAsync();
            if (result.Succeeded)
            {

                // Add alert
                _alerter.Success(T["Sample Entity Categories Added Successfully!"]);

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
        public async Task<IActionResult> UninstallEntityCategories()
        {

            var result = await _sampleEntityCategoriesService.UninstallAsync();
            if (result.Succeeded)
            {

                // Add alert
                _alerter.Success(T["Sample Entity Categories Removed Successfully!"]);

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
