﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Docs.Models;
using Plato.Docs.Tags.Models;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Tags.Stores;
using Plato.Entities.ViewModels;
using PlatoCore.Data.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout;
using PlatoCore.Layout.Titles;
using PlatoCore.Navigation.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.ViewModels;
using Microsoft.AspNetCore.Routing;
using PlatoCore.Hosting.Web.Abstractions;

namespace Plato.Docs.Tags.Controllers
{

    public class HomeController : Controller, IUpdateModel
    {
        
        private readonly IViewProviderManager<Tag> _tagViewProvider;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IPageTitleBuilder _pageTitleBuilder;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;
        private readonly ITagStore<TagBase> _tagStore;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IViewProviderManager<Tag> tagViewProvider,
            IBreadCrumbManager breadCrumbManager,
            IPageTitleBuilder pageTitleBuilder,
            IContextFacade contextFacade,
            IFeatureFacade featureFacade,
            ITagStore<TagBase> tagStore,            
            IAlerter alerter)
        {

            _breadCrumbManager = breadCrumbManager;
            _pageTitleBuilder = pageTitleBuilder;
            _tagViewProvider = tagViewProvider;
            _contextFacade = contextFacade;
            _featureFacade = featureFacade;
            _tagStore = tagStore;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<IActionResult> Index(TagIndexOptions opts, PagerOptions pager)
        {

            // Default options
            if (opts == null)
            {
                opts = new TagIndexOptions();
            }

            // Default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Get default options
            var defaultViewOptions = new TagIndexOptions();
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
            HttpContext.Items[typeof(TagIndexViewModel<Tag>)] = viewModel;

            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetTags", viewModel);
            }

            // Return Url for authentication purposes
            ViewData["ReturnUrl"] = _contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Docs.Tags",
                ["controller"] = "Home",
                ["action"] = "Index"
            });

            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Docs"], docs => docs
                    .Action("Index", "Home", "Plato.Docs")
                    .LocalNav()
                ).Add(S["Tags"]);
            });

            // Return view
            return View((LayoutViewModel) await _tagViewProvider.ProvideIndexAsync(new Tag(), this));

        }
        
        public async Task<IActionResult> Display(EntityIndexOptions opts, PagerOptions pager)
        {
            
            // Default options
            if (opts == null)
            {
                opts = new EntityIndexOptions();
            }

            // Default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Get tag
            var tag = await _tagStore.GetByIdAsync(opts.TagId);

            // Ensure tag exists
            if (tag == null)
            {
                return NotFound();
            }
            
            // Get default options
            var defaultViewOptions = new EntityIndexOptions();
            var defaultPagerOptions = new PagerOptions();

            // Add non default route data for pagination purposes
            if (opts.Search != defaultViewOptions.Search)
                this.RouteData.Values.Add("opts.search", opts.Search);
            if (opts.Sort != defaultViewOptions.Sort)
                this.RouteData.Values.Add("opts.sort", opts.Sort);
            if (opts.Order != defaultViewOptions.Order)
                this.RouteData.Values.Add("opts.order", opts.Order);
            if (opts.Filter != defaultViewOptions.Filter)
                this.RouteData.Values.Add("opts.filter", opts.Filter);
            if (pager.Page != defaultPagerOptions.Page)
                this.RouteData.Values.Add("pager.page", pager.Page);
            if (pager.Size != defaultPagerOptions.Size)
                this.RouteData.Values.Add("pager.size", pager.Size);

            // Build view model
            var viewModel =  await GetDisplayViewModelAsync(opts, pager);

            // Add view model to context
            HttpContext.Items[typeof(EntityIndexViewModel<Doc>)] = viewModel;

            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetDocs", viewModel);
            }

            // Return Url for authentication purposes
            ViewData["ReturnUrl"] = _contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Docs.Tags",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.tagId"] = tag != null ? tag.Id.ToString() : "",
                ["opts.alias"] = tag != null ? tag.Alias.ToString() : ""
            });

            // Build page title
            _pageTitleBuilder.AddSegment(S[tag.Name], int.MaxValue);

            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Docs"], docs => docs
                    .Action("Index", "Home", "Plato.Docs")
                    .LocalNav()
                ).Add(S["Tags"], tags => tags
                    .Action("Index", "Home", "Plato.Docs.Tags")
                    .LocalNav()
                ).Add(S[tag.Name]);
            });

            // Return view
            return View((LayoutViewModel) await _tagViewProvider.ProvideDisplayAsync(new Tag(tag), this));

        }

        // ---------------

        async Task<TagIndexViewModel<Tag>> GetIndexViewModelAsync(TagIndexOptions options, PagerOptions pager)
        {

            // Get current feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Docs");

            // Restrict results to current feature
            if (feature != null)
            {
                options.FeatureId = feature.Id;
            }

            if (options.Sort == TagSortBy.Auto)
            {
                options.Sort = TagSortBy.Entities;
                options.Order = OrderBy.Desc;
            }
            
            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            return new TagIndexViewModel<Tag>()
            {
                Options = options,
                Pager = pager
            };

        }

        async Task<EntityIndexViewModel<Doc>> GetDisplayViewModelAsync(EntityIndexOptions options, PagerOptions pager)
        {
            
            // Get current feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Docs");

            // Restrict results to current feature
            if (feature != null)
            {
                options.FeatureId = feature.Id;
            }

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));
            
            return new EntityIndexViewModel<Doc>()
            {
                Options = options,
                Pager = pager
            };

        }

    }

}
