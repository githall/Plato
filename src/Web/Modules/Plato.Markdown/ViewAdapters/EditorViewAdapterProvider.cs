using System;
using System.Threading.Tasks;
using PlatoCore.Layout.ViewAdapters;

namespace Plato.Markdown.ViewAdapters
{
    public class EditorViewAdapterProvider : ViewAdapterProviderBase
    {
            
        public EditorViewAdapterProvider()
        {
            ViewName = "Editor";
        }

        public override Task<IViewAdapterResult> ConfigureAsync(string viewName)
        {

            if (!viewName.Equals(ViewName, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(default(IViewAdapterResult));
            }
            
            return AdaptAsync(ViewName, v =>
            {
                v.AdaptView("Markdown");
            });
        }

    }

}
