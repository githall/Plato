﻿using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using PlatoCore.Abstractions.Settings;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Models.Users;
using PlatoCore.Stores.Abstractions.Settings;
using PlatoCore.Stores.Abstractions.Users;

namespace PlatoCore.Hosting.Web
{
    public class ContextFacade : IContextFacade
    {

        public const string DefaultCulture = "en-US";

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public ContextFacade(
            IActionContextAccessor actionContextAccessor,
            IHttpContextAccessor httpContextAccessor,
            IPlatoUserStore<User> platoUserStore,
            ISiteSettingsStore siteSettingsStore,
            IUrlHelperFactory urlHelperFactory)
        {
            _actionContextAccessor = actionContextAccessor;
            _httpContextAccessor = httpContextAccessor;
            _siteSettingsStore = siteSettingsStore;
            _urlHelperFactory = urlHelperFactory;
            _platoUserStore = platoUserStore;
        }

        public async Task<User> GetAuthenticatedUserAsync()
        {
            return await GetAuthenticatedUserAsync(_httpContextAccessor.HttpContext.User?.Identity);
        }

        public async Task<User> GetAuthenticatedUserAsync(IIdentity identity)
        {

            if (identity == null)
            {
                return null;
            }

            if (!identity.IsAuthenticated)
            {
                return null;
            }

            if (string.IsNullOrEmpty(identity.Name))
            {
                return null;
            }

            return await _platoUserStore.GetByUserNameAsync(identity.Name);
         
        }

        public async Task<ISiteSettings> GetSiteSettingsAsync()
        {
            return await _siteSettingsStore.GetAsync();
        }

        public async Task<string> GetBaseUrlAsync()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            return await GetBaseUrlAsync(request);
        }

        public async Task<string> GetBaseUrlAsync(HttpRequest request)
        {

            var settings = await GetSiteSettingsAsync();
            if (!String.IsNullOrWhiteSpace(settings?.BaseUrl))
            {
                // trim tailing forward slash
                var lastSlash = settings.BaseUrl.LastIndexOf('/');
                return (lastSlash > -1)
                    ? settings.BaseUrl.Substring(0, lastSlash)
                    : settings.BaseUrl;
            }

            return $"{request.Scheme}://{request.Host}{request.PathBase}";

        }

        public string GetRouteUrl(RouteValueDictionary routeValues)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            return urlHelper.RouteUrl(new UrlRouteContext {Values = routeValues});
        }

        public async Task<string> GetCurrentCultureAsync()
        {

            // Get application culture
            var settings = await GetSiteSettingsAsync();
            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.Culture))
                {
                    return settings.Culture;
                }
            }

            // Return our default culture
            return DefaultCulture;

        }

        public async Task<string> GetCurrentCultureAsync(IIdentity identity)
        {

            // Get users culture
            var user = await GetAuthenticatedUserAsync(identity);
            if (user != null)
            {
                if (!String.IsNullOrEmpty(user.Culture))
                {
                    return user.Culture;
                }
                
            }

            // Get application culture
            var settings = await GetSiteSettingsAsync();
            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.Culture))
                {
                    return settings.Culture;
                }
            }
       
            // Return our default culture
            return DefaultCulture;

        }

        public async Task<string> GetCurrentThemeAsync()
        {

            var settings = await _siteSettingsStore.GetAsync();
            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.Theme))
                {
                    return settings.Theme.ToLower();
                }
            }

            return string.Empty;

        }

    }

}
