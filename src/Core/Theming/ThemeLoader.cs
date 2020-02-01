using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PlatoCore.Abstractions.Settings;
using PlatoCore.FileSystem.Abstractions;
using PlatoCore.Theming.Abstractions;
using PlatoCore.Theming.Abstractions.Locator;
using PlatoCore.Theming.Abstractions.Models;

namespace PlatoCore.Theming
{

    public class ThemeLoader : IThemeLoader
    {

        private IEnumerable<IThemeDescriptor> _themeDescriptors;

        private readonly IThemeLocator _themeLocator;

        public ThemeLoader(
            IHostEnvironment hostingEnvironment,
            IOptions<ThemeOptions> themeOptions,            
            IPlatoFileSystem platoFileSystem,
            IThemeLocator themeLocator)
        {

            _themeLocator = themeLocator;

            var contentRootPath = hostingEnvironment.ContentRootPath;
            var virtualPathToThemesFolder = themeOptions.Value.VirtualPathToThemesFolder;

            RootPath = platoFileSystem.Combine(
                contentRootPath,
                virtualPathToThemesFolder);

            InitializeThemes();
        }

        #region "Implementation"

        public string RootPath { get; private set; }

        public IEnumerable<IThemeDescriptor> AvailableThemes
        {
            get
            {
                InitializeThemes();
                return _themeDescriptors;
            }
        }

        #endregion

        #region "Private Methods"

        void InitializeThemes()
        {         
            _themeDescriptors = new List<IThemeDescriptor>();
            LoadThemeDescriptors();       
        }

        void LoadThemeDescriptors()
        {
            _themeDescriptors = _themeLocator.LocateThemes(
                new string[] { RootPath },
                "Themes",
                "theme.txt",
                false);
        }

        #endregion

    }

}
