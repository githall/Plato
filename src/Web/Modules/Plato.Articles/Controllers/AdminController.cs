﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Articles.Models;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Stores.Abstractions.Settings;
using PlatoCore.Layout.Alerts;

namespace Plato.Articles.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<AdminIndex> _viewProvider;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade, 
            IViewProviderManager<AdminIndex> viewProvider,
            IBreadCrumbManager breadCrumbManager,
            IAlerter alerter)
        {

            _breadCrumbManager = breadCrumbManager;
            _viewProvider = viewProvider;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<IActionResult> Index()
        {
            _breadCrumbManager.Configure(builder =>
            {
                builder
                    .Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Articles"], articles => articles
                        .LocalNav());
            });

            // Return view
            return View(await _viewProvider.ProvideIndexAsync(new AdminIndex(), this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost()
        {

            // Execute view providers
            await _viewProvider.ProvideUpdateAsync(new AdminIndex(), this);

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
