using PlatoCore.Models.Shell;
using PlatoCore.Shell.Abstractions;

namespace PlatoCore.Hosting.Abstractions
{
    public interface IPlatoHost
    {
        void Initialize();

        ShellContext GetOrCreateShellContext(IShellSettings settings);

        void UpdateShellSettings(IShellSettings settings);

        ShellContext CreateShellContext(IShellSettings settings);

        void RecycleShellContext(IShellSettings settings);

        void DisposeShellContext(IShellSettings settings);

    }
}
