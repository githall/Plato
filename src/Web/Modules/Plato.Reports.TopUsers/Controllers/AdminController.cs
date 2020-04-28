﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Stores.Abstractions.Settings;
using Plato.Reports.Models;
using Plato.Reports.ViewModels;
using Plato.Reports.TopUsers.Models;
using PlatoCore.Hosting.Web.Abstractions;

namespace Plato.Reports.TopUsers.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {
        
        private readonly IViewProviderManager<FeatureViewIndex> _pageViewsViewProvider;
     
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IViewProviderManager<FeatureViewIndex> pageViewsViewProvider,
            IViewProviderManager<ReportIndex> reportViewProvider,
            IBreadCrumbManager breadCrumbManager,
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            IFeatureFacade featureFacade,
            IAlerter alerter)
        {
          
            _contextFacade = contextFacade;          
            _breadCrumbManager = breadCrumbManager;
            _alerter = alerter;
            _featureFacade = featureFacade;
            _pageViewsViewProvider = pageViewsViewProvider;

            T = htmlLocalizer;
            S = stringLocalizer;

        }


        public async Task<IActionResult> Index()
        {

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder
                    .Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Reports"], home => home
                        .Action("Index", "Admin", "Plato.Reports")
                        .LocalNav())
                    .Add(S["Top Users"], reports => reports
                        .LocalNav());
            });

            // Return view
            return View((LayoutViewModel) await _pageViewsViewProvider.ProvideIndexAsync(new FeatureViewIndex(), this));

        }
        
        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(ReportOptions opts)
        {

            // Execute view providers
            await _pageViewsViewProvider.ProvideUpdateAsync(new FeatureViewIndex(), this);

            if (!ModelState.IsValid)
            {

                // if we reach this point some view model validation
                // failed within a view provider, display model state errors
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _alerter.Danger(T[error.ErrorMessage]);
                    }
                }

            }

            return await Index();

        }
        
    }

}
