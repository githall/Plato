using System.Collections.Generic;
using PlatoCore.Theming.Abstractions.Models;

namespace Plato.Theming.ViewModels
{
    public class ThemingIndexViewModel
    {

        public IEnumerable<IThemeDescriptor> Themes { get; set; }

    }
}
