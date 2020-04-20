using PlatoCore.Models.Shell;

namespace PlatoCore.Shell.Abstractions
{

    public static class ShellHelper
    {

        public const string DefaultShellName = "Default";

        public const string TablePrefixSeparator = "_";

        public static ShellSettings BuildDefaultUninitializedShell = new ShellSettings
        {
            Name = DefaultShellName,
            State = TenantState.Uninitialized
        };

    }

}
