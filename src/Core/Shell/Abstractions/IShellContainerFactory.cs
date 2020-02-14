using System;
using PlatoCore.Models.Shell;

namespace PlatoCore.Shell.Abstractions
{

    public interface IShellContainerFactory
    {

        IServiceProvider CreateContainer(IShellSettings settings, ShellBlueprint blueprint);

    }

}
