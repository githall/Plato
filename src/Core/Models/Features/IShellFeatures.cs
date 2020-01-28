using System.Collections.Generic;
using PlatoCore.Abstractions;

namespace PlatoCore.Models.Features
{
    public interface IShellFeatures : ISerializable
    {
        IEnumerable<ShellFeature> Features { get; set; }

    }
}
