using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Features;
using Plato.Features.ViewModels;
using Plato.Tour.ViewModels;
using PlatoCore.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Abstractions.Tour;

namespace Plato.Tour.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<FeaturesIndexViewModel> _viewProvider;
        private readonly IShellDescriptorManager _shellDescriptorManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IShellFeatureManager _shellFeatureManager;
        private readonly ITourDescriptorStore _tourDescriptorStore;          
        private readonly IShellSettings _shellSettings;        
        private readonly IPlatoHost _platoHost;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,            
            IViewProviderManager<FeaturesIndexViewModel> viewProvider,
            IShellDescriptorManager shellDescriptorManager,
            IAuthorizationService authorizationService,
            ITourDescriptorStore tourDescriptorStore,
            IShellFeatureManager shellFeatureManager,                
            IShellSettings shellSettings,
            IPlatoHost platoHost,
            IAlerter alerter)
        {

            _shellDescriptorManager = shellDescriptorManager;
            _authorizationService = authorizationService;
            _shellFeatureManager = shellFeatureManager;
            _tourDescriptorStore = tourDescriptorStore;
            _shellSettings = shellSettings;
            _viewProvider = viewProvider;
            _platoHost = platoHost;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;
        }

        // ------------------
        // Enable Features
        // ------------------

        public async Task<IActionResult> Enable(    
            string id,
            string category,
            string returnUrl)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.EnableFeatures))
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));             
            }

            if (string.IsNullOrEmpty(category))
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (string.IsNullOrEmpty(returnUrl))
            {
                throw new ArgumentNullException(nameof(returnUrl));
            }

            // Enable feature
            var result = !string.IsNullOrEmpty(category)
                ? await InstallByCategoryAsync(category)
                : await InstallByIdAsync(id);

            if (result.Succeeded)
            {               
                _alerter.Success(T[$"Features enabled successfully!"]);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                // Redirect to returnUrl
                return RedirectToLocal(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(Index), new RouteValueDictionary()
                {
                    ["opts.category"] = category
                });
            }

        }

        // ------------------
        // Finish set-up
        // ------------------

        public IActionResult FinishSetUp(string returnUrl)
        {

            // Return view
            return View(new FinishSetUpViewModel
            {
                ReturnUrl = returnUrl
            });            

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(FinishSetUp))]
        public async Task<IActionResult> FinishSetUpPost( string returnUrl)
        {

            var descriptor = await _tourDescriptorStore.GetAsync();
            
            if (descriptor == null)
            {
                return NotFound();
            }

            descriptor.Completed = true;

            var result = await _tourDescriptorStore.SaveAsync(descriptor);
            if (result != null)
            {
                // Recycle shell context to ensure changes take effect
                _platoHost.RecycleShell(_shellSettings);

                // Success
                _alerter.Success(T[$"Set-Up Finished Successfully!"]);

            }
            else
            {
                _alerter.Danger(T["A problem occurred ending the set-up assistant!"]);
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                // Redirect to returnUrl
                return RedirectToLocal(returnUrl);
            }

            return Redirect("~/");

        }

        // ------------------------

        private async Task<ICommandResultBase> InstallByCategoryAsync(string categoryName)
        {

            // Our result
            var result = new CommandResultBase();

            // Get all available features
            var features = await _shellDescriptorManager.GetFeaturesAsync();

            // Get all features in feature category
            var categoryFeatures = features?
                .Where(f => f.Descriptor.Category.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => f.Descriptor.Id);

            var errors = new List<CommandError>();
            if (categoryFeatures != null)
            {
                foreach (var feature in categoryFeatures)
                {
                    var contexts = await _shellFeatureManager.EnableFeatureAsync(feature.Descriptor.Id);
                    foreach (var context in contexts)
                    {
                        if (context.Errors.Any())
                        {
                            foreach (var error in context.Errors)
                            {
                                errors.Add(new CommandError($"{context.Feature.ModuleId} could not be enabled. {error.Key} - {error.Value}"));
                            }
                        }
                    }
                }
            }

            return errors.Count > 0
                ? result.Failed(errors.ToArray())
                : result.Success();

        }

        private async Task<ICommandResultBase> InstallByIdAsync(string id)
        {
            var result = new CommandResultBase();

            var errors = new List<CommandError>();
            var contexts = await _shellFeatureManager.EnableFeatureAsync(id);
            foreach (var context in contexts)
            {
                if (context.Errors.Any())
                {
                    foreach (var error in context.Errors)
                    {
                       errors.Add(new CommandError($"{context.Feature.ModuleId} could not be enabled. {error.Key} - {error.Value}"));
                    }
                }
             
            }

            return errors.Count > 0
                ? result.Failed(errors.ToArray())
                : result.Success();

        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("~/");
            }
        }

    }

}
