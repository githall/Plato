using PlatoCore.Models.Shell;
using PlatoCore.Shell.Abstractions;

namespace PlatoCore.Hosting.Abstractions
{
    public interface IPlatoHost
    {
        void Initialize();

        ShellContext GetOrCreateShellContext(IShellSettings settings);

        ShellContext CreateShellContext(IShellSettings settings);

        IPlatoHost UpdateShell(IShellSettings settings);

        IPlatoHost RecycleShell(IShellSettings settings);

        IPlatoHost DisposeShell(IShellSettings settings);

    }
}
