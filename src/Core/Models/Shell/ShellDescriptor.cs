using System.Collections.Generic;
using PlatoCore.Abstractions;

namespace PlatoCore.Models.Shell
{

    public interface IShellDescriptor : ISerializable
    {
        IList<ShellModule> Modules { get; set; }

    }

    public class ShellDescriptor : Serializable, IShellDescriptor
    {

        public IList<ShellModule> Modules { get; set; } = new List<ShellModule>();

    }

}
