using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Features;
using Plato.Features.ViewModels;
using PlatoCore.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.Titles;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Abstractions.Tour;

namespace Plato.Tour.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {

        private readonly ITourDescriptorStore _tourDescriptorStore;
        private readonly IViewProviderManager<FeaturesIndexViewModel> _viewProvider;
        private readonly IShellDescriptorManager _shellDescriptorManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IShellFeatureManager _shellFeatureManager;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IPageTitleBuilder _pageTitleBuilder;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IShellFeatureManager shellFeatureManager,
            IViewProviderManager<FeaturesIndexViewModel> viewProvider, 
            IBreadCrumbManager breadCrumbManager,
            IAuthorizationService authorizationService,
            IShellDescriptorManager shellDescriptorManager,
            ITourDescriptorStore tourDescriptorStore,
            IPageTitleBuilder pageTitleBuilder,
            IAlerter alerter)
        {
            _shellFeatureManager = shellFeatureManager;
            _viewProvider = viewProvider;
            _breadCrumbManager = breadCrumbManager;
            _authorizationService = authorizationService;
            _shellDescriptorManager = shellDescriptorManager;
            _tourDescriptorStore = tourDescriptorStore;
            _pageTitleBuilder = pageTitleBuilder;            
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;
        }

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
                if (string.IsNullOrEmpty(category))
                {
                    throw new ArgumentNullException(nameof(category));
                }
                else
                {
                    throw new ArgumentNullException(nameof(id));
                }          
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

        async Task<ICommandResultBase> InstallByCategoryAsync(string categoryName)
        {

            // Our result
            var result = new CommandResultBase();

            // Get all available features
            var features = await _shellDescriptorManager.GetFeaturesAsync();

            // Get all features in feature category
            var categoryFeatures = features?.Where(f => f.Descriptor.Category.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

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

        async Task<ICommandResultBase> InstallByIdAsync(string id)
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

        IActionResult RedirectToLocal(string returnUrl)
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
