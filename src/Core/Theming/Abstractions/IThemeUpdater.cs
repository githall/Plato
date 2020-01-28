using PlatoCore.Abstractions;
using PlatoCore.Theming.Abstractions.Models;

namespace PlatoCore.Theming.Abstractions
{
    public interface IThemeUpdater
    {

        ICommandResult<IThemeDescriptor> UpdateTheme(string pathToThemeFolder, IThemeDescriptor descriptor);

    }
}
