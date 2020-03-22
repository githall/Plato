using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Site.ViewComponents
{

    public class SiteCallToActionViewComponent : ViewComponent
    {        
 
        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult((IViewComponentResult) View());
        }

    }

}
