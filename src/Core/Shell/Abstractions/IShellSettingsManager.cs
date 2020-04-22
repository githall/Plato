using System.Collections.Generic;
using PlatoCore.Models.Shell;

namespace PlatoCore.Shell.Abstractions
{
    public interface IShellSettingsManager
    {

        IEnumerable<ShellSettings> LoadSettings();

        bool SaveSettings(IShellSettings settings);

        bool DeleteSettings(IShellSettings settings);

    }

}
