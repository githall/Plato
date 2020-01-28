using System;
using System.Linq;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Abstractions;
using PlatoCore.FileSystem.Abstractions;
using PlatoCore.Theming.Abstractions;
using PlatoCore.Theming.Abstractions.Models;

namespace PlatoCore.Theming
{
    
    public class ThemeCreator : IThemeCreator
    {
        
        private readonly ISiteThemeLoader _siteThemeLoader;
        private readonly IPlatoFileSystem _platoFileSystem;
        private readonly IThemeUpdater _themeUpdater;
        public readonly IThemeLoader _themeLoader;
     
        public ThemeCreator(
            ISiteThemeLoader siteThemeLoader,
            IPlatoFileSystem platoFileSystem,            
            IThemeUpdater themeUpdater,
            IThemeLoader themeLoader)
        {
            _siteThemeLoader = siteThemeLoader;
            _platoFileSystem = platoFileSystem;
            _themeUpdater = themeUpdater;
            _themeLoader = themeLoader;
        }

        public ICommandResult<IThemeDescriptor> CreateTheme(string sourceThemeId, string newThemeName)
        {

            // Create result
            var result = new CommandResult<ThemeDescriptor>();

            // Get base theme 
            var baseDescriptor =
                _themeLoader.AvailableThemes.FirstOrDefault(t =>
                    t.Id.Equals(sourceThemeId, StringComparison.OrdinalIgnoreCase));

            // Ensure base theme exists
            if (baseDescriptor == null)
            {
                throw new Exception($"Could not locate the base theme \"{sourceThemeId}\".");
            }

            try
            {

                var newThemeId = newThemeName.ToSafeFileName();
                if (!string.IsNullOrEmpty(newThemeId))
                {
                    newThemeId = newThemeId.ToLower()
                        .Replace(" ", "-");
                }

                // Path to the new directory for our theme
                var targetPath = _platoFileSystem.Combine(
                    _siteThemeLoader.RootPath, newThemeId);

                // Copy base theme to new directory within /Sites/{SiteName}/themes/
                _platoFileSystem.CopyDirectory(
                    baseDescriptor.FullPath,
                    targetPath,
                    true);

                // Update theme name 
                baseDescriptor.Name = newThemeName;
                baseDescriptor.FullPath = targetPath;

                // Update YAML manifest
                var update = _themeUpdater.UpdateTheme(targetPath, baseDescriptor);
                if (!update.Succeeded)
                {
                    return result.Failed(update.Errors.ToArray());
                }

            }
            catch (Exception e)
            {
                return result.Failed(e.Message);
            }

            return result.Success(baseDescriptor);

        }

    }

}
