using System.Threading.Tasks;
using PlatoCore.Models.Shell;

namespace PlatoCore.Shell.Abstractions
{

    public interface ICompositionStrategy
    {
        Task<ShellBlueprint> ComposeAsync(IShellSettings settings, IShellDescriptor descriptor);
    }

}
