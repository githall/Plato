using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using PlatoCore.Abstractions;
using PlatoCore.Abstractions.Settings;
using PlatoCore.FileSystem.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Theming.Abstractions;
using PlatoCore.Theming.Abstractions.Locator;
using PlatoCore.Theming.Abstractions.Models;
using PlatoCore.Yaml;

namespace Plato.Theming.Services
{

    public class SiteThemeLoader : ISiteThemeLoader
    {
        
        private IEnumerable<IThemeDescriptor> _themeDescriptors;

        private readonly IPlatoFileSystem _platoFileSystem;
        private readonly IThemeLocator _themeLocator;
        
        public SiteThemeLoader(
            IOptions<ThemeOptions> themeOptions,
            IPlatoFileSystem platoFilesystem,
            IShellSettings shellSettings,
            IThemeLocator themeLocator,
            ISitesFolder sitesFolder)
        {

            _platoFileSystem = platoFilesystem;
            _themeLocator = themeLocator;

            RootPath = platoFilesystem.Combine(
                sitesFolder.RootPath,
                shellSettings.Location,
                themeOptions.Value.VirtualPathToThemesFolder?.ToLower()); ;

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
            if (_themeDescriptors == null)
            {
                _themeDescriptors = new List<IThemeDescriptor>();
                LoadThemeDescriptors();
            }
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
