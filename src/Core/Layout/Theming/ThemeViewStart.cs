using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Layout.Razor;
using PlatoCore.Theming.Abstractions;

namespace PlatoCore.Layout.Theming
{
    public class ThemeViewStart : RazorPage<dynamic>
    {

        public override Task ExecuteAsync()
        {

            // Compute layout based on controller type
            var themeSelector = Context.RequestServices.GetService<IThemeSelector>();    
            switch (ViewContext.RouteData.Values["controller"].ToString())
            {
                case "Admin":
                    Layout = $"~/{themeSelector.GetThemePath()}/Shared/_AdminLayout.cshtml";
                    break;
                default:
                    Layout = $"~/{themeSelector.GetThemePath()}/Shared/_Layout.cshtml";
                    break;
            }

            return Task.CompletedTask;

        }

    }

}
