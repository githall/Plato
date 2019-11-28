using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Theming.Abstractions;

namespace Plato.Internal.Theming
{

    public class ThemeSelector : IThemeSelector
    {

        private const string HeaderName = "X-Plato-Theme";

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

            // Search headers for a valid default theme
            var theme = GetThemeSetViaRequestHeader();
            if (!string.IsNullOrEmpty(theme))
            {
                return _themeOptions.VirtualPathToThemesFolder + theme;
            }

            // Site theme
            if (!string.IsNullOrEmpty(_siteOptions.Theme))
            {
                return _siteOptions.Theme;
            }

            // Default theme
            return _themeOptions.VirtualPathToThemesFolder + LightThemeFolder;

        }
        
        string GetThemeSetViaRequestHeader()
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

    }

}
