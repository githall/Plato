using System.Collections.Generic;
using PlatoCore.Abstractions;
using PlatoCore.Theming.Abstractions.Models;

namespace PlatoCore.Theming.Abstractions
{
    public interface IThemeLoader
    {
        string RootPath { get;  }

        IEnumerable<IThemeDescriptor> AvailableThemes { get; }
    
    }

}
