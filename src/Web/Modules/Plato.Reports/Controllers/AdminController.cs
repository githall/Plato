﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Layout;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Stores.Abstractions.Settings;
using Plato.Metrics.Models;
using Plato.Reports.Models;
using Plato.Reports.ViewModels;

namespace Plato.Reports.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {
        
        private readonly IViewProviderManager<ReportIndex> _reportViewProvider;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IViewProviderManager<ReportIndex> reportViewProvider,
            IBreadCrumbManager breadCrumbManager,
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            IFeatureFacade featureFacade,
            IAlerter alerter)
        {
          
            _contextFacade = contextFacade;
            _reportViewProvider = reportViewProvider;
            _breadCrumbManager = breadCrumbManager;
            _alerter = alerter;
            _featureFacade = featureFacade;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<IActionResult> Index(ReportOptions opts, PagerOptions pager)
        {

            // Default options
            if (opts == null)
            {
                opts = new ReportOptions();
            }

            // Default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }


            // Get default options
            var defaultViewOptions = new ReportOptions();
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
            var viewModel = await GetIndexViewModelAsync(opts, pager);

            // Add view model to context
            HttpContext.Items[typeof(ReportIndexViewModel<Metric>)] = viewModel;

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder
                    .Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Reports"], reports => reports
                        .LocalNav());
            });

            // Return view
            return View((LayoutViewModel) await _reportViewProvider.ProvideIndexAsync(new ReportIndex(), this));
            
        }
        
        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(ReportOptions opts)
        {

            // Execute view providers
            await _reportViewProvider.ProvideUpdateAsync(new ReportIndex(), this);

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

            return await Index(opts, new PagerOptions()
            {
                Page = 1
            });

        }

        //// ----------------------

        Task<ReportIndexViewModel<Metric>> GetIndexViewModelAsync(ReportOptions options, PagerOptions pager)
        {
            
            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            // Return updated model
            return Task.FromResult(new ReportIndexViewModel<Metric>()
            {
                Options = options,
                Pager = pager
            });

        }

    }

}
