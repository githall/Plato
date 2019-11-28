using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Layout.Razor;
using Plato.Internal.Theming.Abstractions;

namespace Plato.Internal.Layout.Theming
{
    public class ThemeViewStart : RazorPage<dynamic>
    {

        public override Task ExecuteAsync()
        {

            // Compute layout based on controller type

            var themeSelector = Context.RequestServices.GetService<IThemeSelector>();
            var controllerName = ViewContext.RouteData.Values["controller"].ToString();
            switch (controllerName)
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
