using PlatoCore.Models.Shell;

namespace PlatoCore.Shell.Abstractions
{

    public static class ShellHelper
    {

        /// <summary>
        /// The default name used for the first shell or tenant created during initial set-up.
        /// </summary>
        public const string DefaultShellName = "Default";

        public const string TablePrefixSeparator = "_";

        public static ShellSettings BuildDefaultUninitializedShell = new ShellSettings
        {
            Name = DefaultShellName,
            State = TenantState.Uninitialized
        };

    }

}
