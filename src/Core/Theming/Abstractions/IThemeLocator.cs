using System;
using System.Collections.Generic;
using PlatoCore.Theming.Abstractions.Models;

namespace PlatoCore.Theming.Abstractions.Locator
{
    public interface IThemeLocator
    {

        IEnumerable<IThemeDescriptor> LocateThemes(IEnumerable<string> paths, string moduleType, string manifestName, bool manifestIsOptional);

    }
}
