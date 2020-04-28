﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Abstractions.Settings;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Stores.Abstractions.Settings;

namespace PlatoCore.Hosting.Web
{

    /// <summary>
    /// Allows the use of IUrlHelperFactory outside of the ASP.NET core application context.
    /// IUrlHelperFactory requires an ActionContext which is not available within IBackgroundTaskProviders
    /// </summary>
    public class CapturedRouterUrlHelper  : ICapturedRouterUrlHelper
    {

        private readonly IServiceCollection _applicationServices;
        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IUrlHelperFactory _urlHelperFactory;        
        private readonly ICapturedRouter _capturedRouter;

        private IUrlHelper _urlHelper;

        public CapturedRouterUrlHelper(
            IServiceCollection applicationServices,
            ISiteSettingsStore siteSettingsStore,
            IUrlHelperFactory urlHelperFactory,
            ICapturedRouter capturedRouter)
        {
            _applicationServices = applicationServices;
            _siteSettingsStore = siteSettingsStore;
            _urlHelperFactory = urlHelperFactory;
            _capturedRouter = capturedRouter;
        }
        
        public async Task<string> GetBaseUrlAsync()
        {

            // Attempt to get baseUri from site settings
            var settings = await GetSiteSettingsAsync();
            if (!String.IsNullOrWhiteSpace(settings.BaseUrl))
            {
                // trim tailing forward slash
                var lastSlash = settings.BaseUrl.LastIndexOf('/');
                return (lastSlash > -1)
                    ? settings.BaseUrl.Substring(0, lastSlash)
                    : settings.BaseUrl;
            }

            if (_capturedRouter == null)
            {
                throw new ArgumentNullException(nameof(_capturedRouter));
            }

            // Fallback to baseUri within our captured router
            var isNullOrEmpty = String.IsNullOrEmpty(_capturedRouter.Options.BaseUrl);
            var isDefault = _capturedRouter.Options.BaseUrl?.ToLower() == "http://";
            if (isNullOrEmpty | isDefault)
            {
                throw new Exception(
                    "No BaseUrl has been captured. You must configure a BaseUrl via the ICapturedRouter.Configure method before calling GetBaseUrlAsync.");
            }

            return _capturedRouter.Options.BaseUrl;

        }

        public string GetRouteUrl(string baseUri, RouteValueDictionary routeValues)
        {
            return GetRouteUrl(new Uri(baseUri), routeValues);
        }

        public string GetRouteUrl(Uri baseUri, RouteValueDictionary routeValues)
        {

            if (_capturedRouter == null)
            {
                throw new ArgumentNullException(nameof(_capturedRouter));
            }

            if (_capturedRouter.Options.Router == null)
            {
                throw new Exception(
                    "No router has been captured. You must configure a router via the ICapturedRouter.Configure method before calling GetRouteUrl.");
            }

            var httpContext = new DefaultHttpContext()
            {
                RequestServices = _applicationServices.BuildServiceProvider(),
                Request =
                {
                    Scheme = baseUri.Scheme,
                    Host = HostString.FromUriComponent(baseUri),
                    PathBase = PathString.FromUriComponent(baseUri),
                },
            };

            var actionContext = new ActionContext
            {
                HttpContext = httpContext,
                RouteData = new RouteData { Routers = { _capturedRouter.Options.Router } },
                ActionDescriptor = new ActionDescriptor(),
            };

            if (_urlHelper == null)
            {
                _urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);
            }

            return _urlHelper.RouteUrl(new UrlRouteContext { Values = routeValues });

        }

        async Task<ISiteSettings> GetSiteSettingsAsync()
        {
            return await _siteSettingsStore.GetAsync();
        }

    }

}
