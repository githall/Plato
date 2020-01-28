using PlatoCore.Abstractions;
using PlatoCore.Theming.Abstractions.Models;

namespace PlatoCore.Theming.Abstractions
{
    public interface IThemeCreator
    {
        ICommandResult<IThemeDescriptor> CreateTheme(string baseThemeId, string newThemeName);

    }

}
