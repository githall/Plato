using System;
using PlatoCore.Models.Shell;

namespace PlatoCore.Shell
{
    public interface IShellContainerFactory
    {
        IServiceProvider CreateContainer(IShellSettings settings, ShellBlueprint blueprint);
        
    }

}
