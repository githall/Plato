using System.Collections.Generic;
using PlatoCore.Theming.Abstractions;
using PlatoCore.Theming.Abstractions.Models;

namespace PlatoCore.Theming
{
    public class DummySiteThemeLoader : ISiteThemeLoader
    {

        private readonly IThemeLoader _themeLoader;

        public DummySiteThemeLoader(IThemeLoader themeLoader)
        {
            _themeLoader = themeLoader;
        }

        public string RootPath => _themeLoader.RootPath;

        public IEnumerable<IThemeDescriptor> AvailableThemes => _themeLoader.AvailableThemes;

    }

}
