﻿using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using PlatoCore.Layout.Titles;
using PlatoCore.Layout.Views.Abstractions;
using PlatoCore.Models.Users;
using PlatoCore.Hosting.Web.Abstractions;

namespace PlatoCore.Layout.Razor
{

    public abstract class RazorPage<TModel> :
        Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
    
        private IViewDisplayHelper _viewDisplayHelper;
        private IPageTitleBuilder _pageTitleBuilder;
        private IViewLocalizer _t;
        private User _currentUser;

        public User CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    // Attempt to get user set via our AuthenticatedUserMiddleware
                    var user = ViewContext.HttpContext.Features[typeof(User)];
                    _currentUser = (User) user;
                }

                return _currentUser;
            }
        }

        public IViewLocalizer T
        {
            get
            {
                if (_t == null)
                {
                    _t = ViewContext.HttpContext.RequestServices.GetRequiredService<IViewLocalizer>();
                    ((IViewContextAware) _t).Contextualize(ViewContext);
                }
                return _t;
            }
        }

        public IPageTitleBuilder Title =>
            _pageTitleBuilder ??
            (_pageTitleBuilder = Context.RequestServices.GetRequiredService<IPageTitleBuilder>());

        public TOptions GetOptions<TOptions>() where TOptions : class, new()
        {
            return Context.RequestServices.GetRequiredService<IOptions<TOptions>>().Value;
        }

        public void AddTitleSegment(LocalizedString segment, int order = 0)
        {
            Title.AddSegment(segment, order);
        }

        public void AddTitleSegment(LocalizedHtmlString segment, int order = 0)
        {
            Title.AddSegment(new LocalizedString(segment.Name, segment.Value), order);
        }

        private void EnsureViewHelper()
        {
            if (_viewDisplayHelper == null)
            {
                var factory = Context.RequestServices.GetService<IViewHelperFactory>();
                _viewDisplayHelper = factory.CreateHelper(ViewContext);
            }
        }

        public async Task<IHtmlContent> DisplayAsync(IView view)
        {
            EnsureViewHelper();
            return await _viewDisplayHelper.DisplayAsync(view);
        }

        public async Task<IHtmlContent> DisplayAsync(IEnumerable<IView> views)
        {

            var builder = new HtmlContentBuilder();
            foreach (var view in views)
            {
                var viewResult = await DisplayAsync(view);
                var htmlContentBuilder = builder.AppendHtml(viewResult);
            }

            return builder;
        }

        public string GetRouteUrl(RouteValueDictionary routeValues)
        {
            var facade = Context.RequestServices.GetService<IContextFacade>();
            return facade.GetRouteUrl(routeValues);
        }
        
        public bool RouteEquals(string area, string controller, string action)
        {
            return RouteValueEquals("area", area)
                   && RouteValueEquals("controller", controller)
                   && RouteValueEquals("action", action);
        }

        public bool RouteValueEquals(string key, string value)
        {

            // We need route values to perform the checks
            if (ViewContext.RouteData.Values == null)
            {
                return false;
            }

            // Ensure the key exists
            if (!ViewContext.RouteData.Values.ContainsKey(key))
            {
                return false;
            }

            // Compare values
            return ViewContext.RouteData.Values[key].ToString().Equals(value, StringComparison.OrdinalIgnoreCase);

        }

    }

}
