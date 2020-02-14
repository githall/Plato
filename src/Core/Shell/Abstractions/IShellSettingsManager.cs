using System.Collections.Generic;
using PlatoCore.Models.Shell;

namespace PlatoCore.Shell.Abstractions
{
    public interface IShellSettingsManager
    {

        IEnumerable<ShellSettings> LoadSettings();

        void SaveSettings(IShellSettings settings);

    }

}
