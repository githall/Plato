using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PlatoCore.Abstractions.Settings;
using PlatoCore.Theming.Abstractions;

namespace PlatoCore.Theming
{

    public class ThemeSelector : IThemeSelector
    {

        private const string HeaderName = "X-Plato-Theme";
        private const string CookieName = "plato-theme";

        private const string LightThemeFolder = "/default";
        private const string DarkThemeFolder = "/dark";

        private readonly ThemeOptions _themeOptions;
        private readonly SiteOptions _siteOptions;
        private readonly HttpContext _httpContext;

        public ThemeSelector(
            IOptions<ThemeOptions> themeOptionsAccessor,
            IOptions<SiteOptions> siteOptionsAccessor,
            IHttpContextAccessor httpContextAccessor)
        {
            _themeOptions = themeOptionsAccessor.Value;
            _siteOptions = siteOptionsAccessor.Value;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public string GetThemePath()
        {

            // Theme can be set via request headers or cookies
            var theme = GetHttpTheme();
            if (!string.IsNullOrEmpty(theme))
            {
                return _themeOptions.VirtualPathToThemesFolder + theme;
            }

            // Use site's configured theme
            if (!string.IsNullOrEmpty(_siteOptions.Theme))
            {
                return _siteOptions.Theme;
            }

            // Use default theme
            return _themeOptions.VirtualPathToThemesFolder + LightThemeFolder;

        }        

        string GetHttpTheme()
        {

            var headerTheme = GetViaHttptHeaders();
            if (!string.IsNullOrEmpty(headerTheme))
            {
                return headerTheme;
            }

            var cookieTheme = GetViaHttpCookies();
            if (!string.IsNullOrEmpty(cookieTheme))
            {
                return cookieTheme;
            }

            return string.Empty;

        }

        string GetViaHttptHeaders()
        {

            if (_httpContext.Request.Headers.ContainsKey(HeaderName))
            {
                var themeName = _httpContext.Request.Headers[HeaderName].ToString();
                switch (themeName.ToLowerInvariant())
                {
                    case "light":
                        return LightThemeFolder;
                    case "dark":
                        return DarkThemeFolder;
                }
            }

            return string.Empty;

        }

        string GetViaHttpCookies()
        {

            if (_httpContext.Request.Cookies.ContainsKey(CookieName))
            {
                var themeName = _httpContext.Request.Cookies[CookieName].ToString();
                switch (themeName.ToLowerInvariant())
                {
                    case "light":
                        return LightThemeFolder;
                    case "dark":
                        return DarkThemeFolder;
                }
            }

            return string.Empty;

        }

    }

}
