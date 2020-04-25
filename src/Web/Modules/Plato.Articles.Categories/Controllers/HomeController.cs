using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Categories.Stores;
using Plato.Articles.Categories.Models;
using Plato.Articles.Models;
using PlatoCore.Hosting.Web.Abstractions;
using Plato.Entities.ViewModels;
using PlatoCore.Data.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.Titles;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Navigation.Abstractions;
using Plato.Categories.Services;
using Plato.Categories.Models;
using Plato.Categories.Extensions;

namespace Plato.Articles.Categories.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {
     
        private readonly IViewProviderManager<Category> _viewProvider;
        private readonly ICategoryService<Category> _categoryService;
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IPageTitleBuilder _pageTitleBuilder;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IViewProviderManager<Category> viewProvider,
            IStringLocalizer<AdminController> stringLocalizer,
            ICategoryService<Category> categoryService,
            IHtmlLocalizer<HomeController> localizer,
            ICategoryStore<Category> categoryStore,
            IBreadCrumbManager breadCrumbManager,
            IPageTitleBuilder pageTitleBuilder,
            IContextFacade contextFacade,
            IFeatureFacade featureFacade,            
            IAlerter alerter)
        {

            _breadCrumbManager = breadCrumbManager;
            _pageTitleBuilder = pageTitleBuilder;
            _categoryService = categoryService;
            _contextFacade = contextFacade;
            _featureFacade = featureFacade;
            _categoryStore = categoryStore;
            _viewProvider = viewProvider;            
            _alerter = alerter;

            T = localizer;
            S = stringLocalizer;
        }

        public async Task<IActionResult> Index(EntityIndexOptions opts, PagerOptions pager)
        {

            // Build options
            if (opts == null)
            {
                opts = new EntityIndexOptions();
            }

            // Build pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            Category category = null;
            if (opts.CategoryId > 0)
            {

                // Get category
                category = await _categoryStore.GetByIdAsync(opts.CategoryId);
                if (category == null)
                {
                    return NotFound();
                }

                // Get the permissioned category
                var permissionedCategory = await GetCategoryAsync(opts.CategoryId);
                if (permissionedCategory == null)
                {
                    return Unauthorized();
                }

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
            var viewModel = await GetIndexViewModelAsync(category, opts, pager);

            // Add view model to context
            HttpContext.Items[typeof(EntityIndexViewModel<Article>)] = viewModel;

            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0 && !pager.Enabled)
                    return View("GetArticles", viewModel);
            }

            // Return Url for authentication purposes
            ViewData["ReturnUrl"] = _contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles.Categories",
                ["controller"] = "Home",
                ["action"] = "Index",
                ["opts.categoryId"] = category != null ? category.Id.ToString() : "",
                ["opts.alias"] = category != null ? category.Alias.ToString() : ""
            });

            // Build page title
            if (category != null)
            {
                _pageTitleBuilder.AddSegment(S[category.Name], int.MaxValue);
            }

            // Build breadcrumb
            _breadCrumbManager.Configure(async builder =>
            {

                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Articles"], home => home
                    .Action("Index", "Home", "Plato.Articles")
                    .LocalNav()
                );

                // Build breadcrumb
                var parents = category != null
                    ? await _categoryStore.GetParentsByIdAsync(category.Id)
                    : null;
                if (parents == null)
                {
                    builder.Add(S["Categories"]);
                }
                else
                {

                    builder.Add(S["Categories"], categories => categories
                        .Action("Index", "Home", "Plato.Articles.Categories", new RouteValueDictionary()
                        {
                            ["opts.categoryId"] = null,
                            ["opts.alias"] = null
                        })
                        .LocalNav()
                    );
                    
                    foreach (var parent in parents)
                    {
                        if (parent.Id != category.Id)
                        {
                            builder.Add(S[parent.Name], c => c
                                .Action("Index", "Home", "Plato.Articles.Categories", new RouteValueDictionary
                                {
                                    ["opts.categoryId"] = parent.Id,
                                    ["opts.alias"] = parent.Alias,
                                })
                                .LocalNav()
                            );
                        }
                        else
                        {
                            builder.Add(S[parent.Name]);
                        }

                    }
                    
                }

            });
            
            // Return view
            return View((LayoutViewModel) await _viewProvider.ProvideIndexAsync(category, this));

        }

        // ---------------

        // Use the category service to get the category to 
        // ensure query adapters are enforced
        private async Task<ICategory> GetCategoryAsync(int categoryId)
        {

            if (categoryId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(categoryId));
            }

            // Get categories feature
            var featureId = 0;
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Articles.Categories");
            if (feature != null)
            {
                featureId = feature.Id;
            }

            // Build categories for feature
            var categories = await _categoryService
                .ConfigureQuery(q =>
                {
                    q.FeatureId.Equals(featureId);
                })
                .GetResultsAsync();

            // Ensure the user has access to the category
            if (categories?.Data != null)
            {
                return categories.Data.GetById<Category>(categoryId);
            }

            return null;

        }

        private async Task<EntityIndexViewModel<Article>> GetIndexViewModelAsync(Category category, EntityIndexOptions options, PagerOptions pager)
        {

            // Include child categories
            if (category != null)
            {
                if (category.Children.Any())
                {
                    // Convert child ids to list and add current id
                    var ids = category
                        .Children
                        .Select(c => c.Id).ToList();
                    ids.Add(category.Id);
                    options.CategoryIds = ids.ToArray();
                }
                else
                {
                    options.CategoryId = category.Id;
                }
            }

            // Get current feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Articles");

            // Restrict results to current feature
            if (feature != null)
            {
                options.FeatureId = feature.Id;
            }

            // Ensure pinned appear first
            if (options.Sort == SortBy.LastReply)
            {
                options.AddSortColumn(SortBy.IsPinned.ToString(), OrderBy.Desc);
            }

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            // Return updated model
            return new EntityIndexViewModel<Article>()
            {
                Options = options,
                Pager = pager
            };

        }

    }

}
