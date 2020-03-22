using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Site.ViewComponents
{

    public class SiteSharedFeaturesViewComponent : ViewComponent
    {        
 
        public Task<IViewComponentResult> InvokeAsync()
        {

            return Task.FromResult((IViewComponentResult) View());
        }

    }

}
