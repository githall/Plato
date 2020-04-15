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
using PlatoCore.Shell.Abstractions;
using System.Linq;

namespace Plato.Tenants.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly IShellSettingsManager _shellSettingsManager;

        private readonly IViewProviderManager<ShellSettings> _viewProvider;
        private readonly IAuthorizationService _authorizationService;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
             IShellSettingsManager shellSettingsManager,
            IViewProviderManager<ShellSettings> viewProvider,
            IAuthorizationService authorizationService,
            IBreadCrumbManager breadCrumbManager,
            IAlerter alerter)
        {
       
            _authorizationService = authorizationService;
            _shellSettingsManager = shellSettingsManager;
            _breadCrumbManager = breadCrumbManager;
            _viewProvider = viewProvider;
            
            _alerter = alerter;

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

            //// Ensure we have permission
            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.AddRoles))
            //{
            //    return Unauthorized();
            //}

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

        [HttpPost, ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost()
        {

            //var newRole = new Role();
            //var roleClaims = new List<RoleClaim>();
            //foreach (string key in Request.Form.Keys)
            //{
            //    if (key.StartsWith("Checkbox.") && Request.Form[key] == "true")
            //    {
            //        var permissionName = key.Substring("Checkbox.".Length);
            //        roleClaims.Add(new RoleClaim { ClaimType = Permission.ClaimTypeName, ClaimValue = permissionName });
            //    }
            //}

            //newRole.RoleClaims.RemoveAll(c => c.ClaimType == Permission.ClaimTypeName);
            //newRole.RoleClaims.AddRange(roleClaims);

            var result = await _viewProvider.ProvideUpdateAsync(new ShellSettings(), this);

            if (!ModelState.IsValid)
            {
                _alerter.Success(T["Tenant Created Successfully!"]);
            }
            else
            {
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _alerter.Danger(T[error.ErrorMessage]);
                    }
                }
            }


            return RedirectToAction(nameof(Index));

        }

        // --------------
        // Edit
        // --------------

        public async Task<IActionResult> Edit(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            //// Ensuer we have permission
            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditRoles))
            //{
            //    return Unauthorized();
            //}

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


            var shell = GetShell(id);
            if (shell == null)
            {
                return NotFound();
            }               

            return View((LayoutViewModel)await _viewProvider.ProvideEditAsync(shell, this));

        }

        [HttpPost, ActionName(nameof(Edit))]
        public async Task<IActionResult> EditPost(string name)
        {

            var shell = GetShell(name);

            //var role = await _roleManager.FindByIdAsync(id);
            //if (role == null)
            //{
            //    return NotFound();
            //}

            //var roleClaims = new List<RoleClaim>();
            //foreach (string key in Request.Form.Keys)
            //{
            //    if (key.StartsWith("Checkbox.") && Request.Form[key] == "true")
            //    {
            //        var permissionName = key.Substring("Checkbox.".Length);
            //        roleClaims.Add(new RoleClaim { ClaimType = Permission.ClaimTypeName, ClaimValue = permissionName });
            //    }
            //}

            //role.RoleClaims.RemoveAll(c => c.ClaimType == Permission.ClaimTypeName);
            //role.RoleClaims.AddRange(roleClaims);

            var result = await _viewProvider.ProvideUpdateAsync(shell, this);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            _alerter.Success(T["Tenant Updated Successfully!"]);

            return RedirectToAction(nameof(Index));

        }


        ShellSettings GetShell(string name)
        {

            var shells = _shellSettingsManager.LoadSettings();
            if (shells == null)
            {
                return null;
            }
       
            return shells.First(s => s.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase));
            
        }

    }

}
