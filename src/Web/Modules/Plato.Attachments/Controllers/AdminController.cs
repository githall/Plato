using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Attachments.Models;
using Plato.Attachments.ViewModels;
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

        public async Task<IActionResult> Index()
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
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(new AttachmentSettings(), this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(AttachmentSettingsViewModel viewModel)
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
