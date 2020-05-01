using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Localization;
using PlatoCore.Layout;
using PlatoCore.Models.Shell;
using PlatoCore.Layout.Alerts;
using PlatoCore.Shell.Abstractions;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Security.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Tenants.Services;
using Plato.Tenants.ViewModels;
using Plato.Tenants.Models;

namespace Plato.Tenants.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<ShellSettings> _viewProvider;
        private readonly IViewProviderManager<DefaultTenantSettings> _settingsViewProvider;

        private readonly IAuthorizationService _authorizationService;
        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly ITenantSetUpService _setUpService;
        private readonly ILogger<AdminController> _logger;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
             IViewProviderManager<DefaultTenantSettings> settingsViewProvider,
            IViewProviderManager<ShellSettings> viewProvider,
            IAuthorizationService authorizationService,
            IShellSettingsManager shellSettingsManager,
            IBreadCrumbManager breadCrumbManager,
            ILogger<AdminController> logger,
            ITenantSetUpService setUpService,
            IAlerter alerter)
        {

            _authorizationService = authorizationService;
            _shellSettingsManager = shellSettingsManager;
            _settingsViewProvider = settingsViewProvider;
            _breadCrumbManager = breadCrumbManager;            
            _viewProvider = viewProvider;
            _setUpService = setUpService;
            _alerter = alerter;
            _logger = logger;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        // --------------
        // Index
        // --------------

        public async Task<IActionResult> Index(TenantIndexOptions opts)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageTenants))
            {
                return Unauthorized();
            }

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
                ).Add(S["Tenants"]);
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

        // --------------
        // Create
        // --------------

        public async Task<IActionResult> Create()
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.AddTenants))
            {
                return Unauthorized();
            }

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Tenants"], tenants => tenants
                    .Action("Index", "Admin", "Plato.Tenants")
                    .LocalNav()
                ).Add(S["Add Tenant"]);
            });

            return View((LayoutViewModel)await _viewProvider.ProvideEditAsync(new ShellSettings(), this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditTenantViewModel model)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.AddTenants))
            {
                return Unauthorized();
            }

            // Execute view provider
            var result = await _viewProvider.ProvideUpdateAsync(new ShellSettings(), this);

            // Errors occurred in the view provider
            if (!ModelState.IsValid)
            {
                return await Create();
            }

            // Success
            _alerter.Success(T["Tenant Created Successfully!"]);
            return RedirectToAction(nameof(Index));

        }

        // --------------
        // Edit
        // --------------

        public async Task<IActionResult> Edit(string id)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditTenants))
            {
                return Unauthorized();
            }

            // Ensure we have an id
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Get shell
            var shell = GetShell(id);

            // Ensure the shell exists
            if (shell == null)
            {
                return NotFound();
            }

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Tenants"], tenants => tenants
                    .Action("Index", "Admin", "Plato.Tenants")
                    .LocalNav()
                ).Add(S["Edit Tenant"]);
            });

            return View((LayoutViewModel)await _viewProvider.ProvideEditAsync(shell, this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Edit))]
        public async Task<IActionResult> EditPost(string id)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditTenants))
            {
                return Unauthorized();
            }

            // Get shell
            var shell = GetShell(id);

            // Ensure the shell exists
            if (shell == null)
            {
                return NotFound();
            }

            // Update shell
            var result = await _viewProvider.ProvideUpdateAsync(shell, this);

            // Errors occurred in the view provider
            if (!ModelState.IsValid)
            {
                return await Edit(id);
            }

            // Success
            _alerter.Success(T["Tenant Updated Successfully!"]);
            return RedirectToAction(nameof(Index));

        }

        // --------------
        // Delete
        // --------------

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.DeleteTenants))
            {
                return Unauthorized();
            }

            // Get shell
            var shell = GetShell(id);

            // Ensure the shell exists
            if (shell == null)
            {
                return NotFound();
            }

            // Attempt to delete the tenant
            var result = await _setUpService.UninstallAsync(id);

            // Redirect to success
            if (result.Succeeded)
            {
                _alerter.Success(T["Tenant Deleted Successfully"]);
                return RedirectToAction(nameof(Index));
            }

            // Display errors
            foreach (var error in result.Errors)
            {
                _alerter.Danger(T[error.Description]);
            }

            return RedirectToAction(nameof(Edit), new RouteValueDictionary()
            {
                ["id"] = id
            });

        }

        // --------------
        // Settings
        // --------------

        public async Task<IActionResult> Settings()
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditSettings))
            {
                return Unauthorized();
            }

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Tenants"], tenants => tenants
                    .Action("Index", "Admin", "Plato.Tenants")
                    .LocalNav()
                ).Add(S["Settings"]);
            });

            return View((LayoutViewModel)await _settingsViewProvider.ProvideEditAsync(new DefaultTenantSettings(), this));

        }


        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Settings))]
        public async Task<IActionResult> SettingsPost(EditTenantSettingsViewModel viewModel)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditSettings))
            {
                return Unauthorized();
            }

            // Execute view providers ProvideUpdateAsync method
            await _settingsViewProvider.ProvideUpdateAsync(new DefaultTenantSettings(), this);

            // Add alert
            _alerter.Success(T["Settings Updated Successfully!"]);

            return RedirectToAction(nameof(Settings));

        }

        // ----------------

        ShellSettings GetShell(string name)
        {      
            return _shellSettingsManager.LoadSettings()?
                .First(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));            
        }

    }

}
