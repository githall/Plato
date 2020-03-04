using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Attachments.Models;
using Plato.Attachments.ViewModels;
using Plato.Roles.ViewModels;
using PlatoCore.Layout;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Attachments.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<AttachmentSettings> _viewProvider;
        private readonly IAuthorizationService _authorizationService;                
        private readonly IBreadCrumbManager _breadCrumbManager;         
        private readonly IAlerter _alerter;
           
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IViewProviderManager<AttachmentSettings> viewProvider,
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

        // ---------------
        // Index
        // ---------------

        public async Task<IActionResult> Index(RoleIndexOptions opts, PagerOptions pager)
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
                ).Add(S["Settings"], channels => channels
                    .Action("Index", "Admin", "Plato.Settings")
                    .LocalNav()
                ).Add(S["Attachments"]);
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
            var viewModel = new AttachmentsIndexViewModel()
            {
                Options = opts,
                Pager = pager
            };

            // Add view model to context
            this.HttpContext.Items[typeof(AttachmentsIndexViewModel)] = viewModel;

            // Return view
            return View((LayoutViewModel) await _viewProvider.ProvideIndexAsync(new AttachmentSettings(), this));

        }

        // ---------------
        // Edit
        // ---------------


        public async Task<IActionResult> Edit(int id)
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
                ).Add(S["Settings"], channels => channels
                    .Action("Index", "Admin", "Plato.Settings")
                    .LocalNav()
                ).Add(S["Attachments"]);
            });

            // Return view
            return View((LayoutViewModel)await _viewProvider.ProvideEditAsync(new AttachmentSettings(), this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Edit))]
        public async Task<IActionResult> EditPost(AttachmentsIndexViewModel viewModel)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAttachmentSettings))
            {
                return Unauthorized();
            }

            // Execute view providers ProvideUpdateAsync method
            await _viewProvider.ProvideUpdateAsync(new AttachmentSettings(), this);

            // Add alert
            _alerter.Success(T["Settings Updated Successfully!"]);

            return RedirectToAction(nameof(Index));

        }

    }

}
