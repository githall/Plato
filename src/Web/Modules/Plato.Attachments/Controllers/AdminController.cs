using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Attachments.Models;
using Plato.Attachments.ViewModels;
using Plato.Roles.ViewModels;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Roles;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Abstractions.Roles;

namespace Plato.Attachments.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<AttachmentIndex> _indexViewProvider;
        private readonly IViewProviderManager<AttachmentSetting> _settingsViewProvider;
        private readonly IAuthorizationService _authorizationService;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IPlatoRoleStore _platoRoleStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IViewProviderManager<AttachmentSetting> settingsViewProvider,
            IViewProviderManager<AttachmentIndex> indexViewProvider,
            IAuthorizationService authorizationService,
            IBreadCrumbManager breadCrumbManager,
            IPlatoRoleStore platoRoleStore,
            IContextFacade contextFacade,
            IFeatureFacade featureFacade,
            IAlerter alerter)
        {

            _authorizationService = authorizationService;
            _settingsViewProvider = settingsViewProvider;
            _indexViewProvider = indexViewProvider;
            _breadCrumbManager = breadCrumbManager;
            _platoRoleStore = platoRoleStore;
            _featureFacade = featureFacade;
            _contextFacade = contextFacade;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        // ---------------
        // Index
        // ---------------

        public async Task<IActionResult> Index(AttachmentIndexOptions opts, PagerOptions pager)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAttachmentSettings))
            {
                return Unauthorized();
            }

            // Default options
            if (opts == null)
            {
                opts = new AttachmentIndexOptions();
            }

            // Default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Get default options
            var defaultViewOptions = new AttachmentIndexOptions();
            var defaultPagerOptions = new PagerOptions();

            // Add non default route data for pagination purposes
            if (opts.Start != defaultViewOptions.Start)
                this.RouteData.Values.Add("opts.start", opts.Start);
            if (opts.End != defaultViewOptions.End)
                this.RouteData.Values.Add("opts.end", opts.End);
            if (opts.Search != defaultViewOptions.Search)
                this.RouteData.Values.Add("opts.search", opts.Search);
            if (opts.Sort != defaultViewOptions.Sort)
                this.RouteData.Values.Add("opts.sort", opts.Sort);
            if (opts.Order != defaultViewOptions.Order)
                this.RouteData.Values.Add("opts.order", opts.Order);
            if (pager.Page != defaultPagerOptions.Page)
                this.RouteData.Values.Add("pager.page", pager.Page);
            if (pager.Size != defaultPagerOptions.Size)
                this.RouteData.Values.Add("pager.size", pager.Size);

            // Build view model
            var viewModel = await GetIndexViewModelAsync(opts, pager);

            // Add view model to context
            HttpContext.Items[typeof(AttachmentIndexViewModel)] = viewModel;

            // If we have a pager.page querystring value return paged view
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetAttachments", viewModel);
            }

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()                
                ).Add(S["Attachments"]);
            });

            // Return view
            return View((LayoutViewModel)await _indexViewProvider.ProvideIndexAsync(new AttachmentIndex(), this));

        }

        // ---------------
        // Settings
        // ---------------

        public async Task<IActionResult> Settings(RoleIndexOptions opts, PagerOptions pager)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAttachmentSettings))
            {
                return Unauthorized();
            }

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Attachments"], manage => manage
                    .Action("Index", "Admin", "Plato.Attachments")
                    .LocalNav()
                ).Add(S["Settings"]);
            });

            // default options
            if (opts == null)
            {
                opts = new RoleIndexOptions();
            }

            // default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Get default options
            var defaultViewOptions = new RoleIndexOptions();
            var defaultPagerOptions = new PagerOptions();

            // Add non default route data for pagination purposes
            if (opts.Search != defaultViewOptions.Search)
                this.RouteData.Values.Add("opts.search", opts.Search);
            if (opts.Sort != defaultViewOptions.Sort)
                this.RouteData.Values.Add("opts.sort", opts.Sort);
            if (opts.Order != defaultViewOptions.Order)
                this.RouteData.Values.Add("opts.order", opts.Order);
            if (pager.Page != defaultPagerOptions.Page)
                this.RouteData.Values.Add("pager.page", pager.Page);
            if (pager.Size != defaultPagerOptions.Size)
                this.RouteData.Values.Add("pager.size", pager.Size);

            // Build view model
            var viewModel = new AttachmentSettingsViewModel()
            {
                Options = opts,
                Pager = pager
            };

            // Add view model to context
            this.HttpContext.Items[typeof(AttachmentSettingsViewModel)] = viewModel;

            // Return view
            return View((LayoutViewModel) await _settingsViewProvider.ProvideIndexAsync(new AttachmentSetting(), this));

        }

        // ---------------
        // EditSettings
        // ---------------

        public async Task<IActionResult> EditSettings(int id)
        {

            if (id <= 0)
            {
                return NotFound();
            }

            // Get role
            var role = await _platoRoleStore.GetByIdAsync(id);
            
            if (role == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAttachmentSettings))
            {
                return Unauthorized();
            }

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Attachments"], manage => manage
                    .Action("Index", "Admin", "Plato.Attachments")
                    .LocalNav()
                ).Add(S["Settings"], attachments => attachments
                    .Action("Settings", "Admin", "Plato.Attachments")
                    .LocalNav()
                ).Add(S[role.Name]);
            });

            // Add view model to context
            this.HttpContext.Items[typeof(Role)] = role;

            // Return view
            return View((LayoutViewModel)await _settingsViewProvider.ProvideEditAsync(new AttachmentSetting(), this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(EditSettings))]
        public async Task<IActionResult> EditSettingsPost(EditAttachmentSettingsViewModel viewModel)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAttachmentSettings))
            {
                return Unauthorized();
            }

            // Get role
            var role = await _platoRoleStore.GetByIdAsync(viewModel.RoleId);

            if (role == null)
            {
                return NotFound();
            }

            // Add view model to context
            this.HttpContext.Items[typeof(Role)] = role;

            // Execute view providers ProvideUpdateAsync method
            await _settingsViewProvider.ProvideUpdateAsync(new AttachmentSetting(), this);

            // Add alert
            _alerter.Success(T["Settings Updated Successfully!"]);

            return RedirectToAction(nameof(EditSettings), new RouteValueDictionary()
            {
                ["id"] = viewModel.RoleId.ToString()
            });

        }

        // -------------------

        async Task<AttachmentIndexViewModel> GetIndexViewModelAsync(AttachmentIndexOptions options, PagerOptions pager)
        {

            // Get current feature
            var feature = await _featureFacade.GetFeatureByIdAsync(RouteData.Values["area"].ToString());

            // Restrict results to current feature
            if (feature != null)
            {
                options.FeatureId = feature.Id;
            }

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            // Return updated model
            return new AttachmentIndexViewModel()
            {
                Options = options,
                Pager = pager
            };

        }


    }

}
