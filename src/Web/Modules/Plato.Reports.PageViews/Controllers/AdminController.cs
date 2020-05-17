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
using Plato.Metrics.Models;
using Plato.Reports.ViewModels;
using Plato.Reports.PageViews.Models;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Abstractions.Extensions;

namespace Plato.Reports.PageViews.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<PageViewIndex> _pageViewsViewProvider;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IViewProviderManager<PageViewIndex> pageViewsViewProvider,
            IBreadCrumbManager breadCrumbManager,
            IContextFacade contextFacade,
            IFeatureFacade featureFacade,
            IAlerter alerter)
        {

            _pageViewsViewProvider = pageViewsViewProvider;
            _breadCrumbManager = breadCrumbManager;
            _featureFacade = featureFacade;
            _contextFacade = contextFacade;
            _alerter = alerter;

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
            if (opts.Start != defaultViewOptions.Start)
                AddRouteValue("opts.start", opts.Start);
            if (opts.End != defaultViewOptions.End)
                AddRouteValue("opts.end", opts.End);
            if (opts.Search != defaultViewOptions.Search)
                AddRouteValue("opts.search", opts.Search);
            if (opts.Sort != defaultViewOptions.Sort)
                AddRouteValue("opts.sort", opts.Sort);
            if (opts.Order != defaultViewOptions.Order)
                AddRouteValue("opts.order", opts.Order);
            if (pager.Page != defaultPagerOptions.Page)
                AddRouteValue("pager.page", pager.Page);
            if (pager.Size != defaultPagerOptions.Size)
                AddRouteValue("pager.size", pager.Size);

            // Build view model
            var viewModel = await GetIndexViewModelAsync(opts, pager);

            // Add view model to context
            HttpContext.Items[typeof(ReportIndexViewModel<Metric>)] = viewModel;

            // If we have a pager.page query string value return paged view
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetMetrics", viewModel);
            }

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
                    .Add(S["Page Views"], reports => reports
                        .LocalNav());
            });

            // Return view
            return View((LayoutViewModel)await _pageViewsViewProvider.ProvideIndexAsync(new PageViewIndex(), this));

        }
        
        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(ReportOptions opts, PagerOptions pager)
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

            // Build view model
            var viewModel = await GetIndexViewModelAsync(opts, pager);

            // Add view model to context
            HttpContext.Items[typeof(ReportIndexViewModel<Metric>)] = viewModel;

            // Execute view providers
            await _pageViewsViewProvider.ProvideUpdateAsync(new PageViewIndex(), this);

            // May have been invalidated by a view provider
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

        // ----------------------

        private void AddRouteValue(string key, object value)
        {
            if (!RouteData.Values.ContainsKey(key))
            {
                RouteData.Values.Add(key, value);
            }
        }

        private async Task<ReportIndexViewModel<Metric>> GetIndexViewModelAsync(ReportOptions options, PagerOptions pager)
        {

            // Get current feature
            var feature = await _featureFacade.GetFeatureByIdAsync(RouteData.Values["area"].ToString());

            // Restrict results to current feature
            if (feature != null)
            {
                options.FeatureId = feature.Id;
            }

            // Set pager call back URL
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            // Return updated model
            return new ReportIndexViewModel<Metric>()
            {
                Options = options,
                Pager = pager
            };

        }

    }

}
