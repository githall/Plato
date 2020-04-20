using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using PlatoCore.Layout;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Navigation.Abstractions;
using Plato.Tenants.ViewModels;
using PlatoCore.Models.Shell;
using PlatoCore.Shell.Abstractions;
using System.Linq;
using Plato.Tenants.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using Plato.Tenants.Models;

namespace Plato.Tenants.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<ShellSettings> _viewProvider;
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
                ).Add(S["Tenants"], channels => channels
                    .Action("Index", "Admin", "Plato.Tenants")
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

        // --------------
        // Create
        // --------------

        public async Task<IActionResult> Create()
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Tenants"], channels => channels
                    .Action("Index", "Admin", "Plato.Tenants")
                    .LocalNav()
                ).Add(S["Add Tenant"]);
            });

            return View((LayoutViewModel)await _viewProvider.ProvideEditAsync(new ShellSettings(), this));
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditTenantViewModel model)
        {

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

            //// Ensure we have permission
            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditRoles))
            //{
            //    return Unauthorized();
            //}

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
                ).Add(S["Tenants"], channels => channels
                    .Action("Index", "Admin", "Plato.Tenants")
                    .LocalNav()
                ).Add(S["Edit Tenant"]);
            });
     
            return View((LayoutViewModel)await _viewProvider.ProvideEditAsync(shell, this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Edit))]
        public async Task<IActionResult> EditPost(string id)
        {

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
            return RedirectToAction(nameof(Edit), new RouteValueDictionary()
            {
                ["id"] = id
            });      

        }

        // --------------
        // Delete
        // --------------

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {

            // Ensure we have permission
            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.DeleteRoles))
            //{
            //    return Unauthorized();
            //}

            // Get shell
            var shell = GetShell(id);

            // Ensure the shell exists
            if (shell == null)
            {
                return NotFound();
            }

            // Attempt to delete the role
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
                ModelState.AddModelError(error.Code, error.Description);
            }

            return await Edit(id);         

        }

        // ----------------

        ShellSettings GetShell(string name)
        {      
            return _shellSettingsManager.LoadSettings()?
                .First(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));            
        }

    }

}
