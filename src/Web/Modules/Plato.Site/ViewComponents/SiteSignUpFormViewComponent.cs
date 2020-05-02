using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Plato.Site.Models;

namespace Plato.Site.ViewComponents
{

    public class SiteSignUpFormViewComponent : ViewComponent
    {

        private readonly PlatoSiteSettings _platoSiteSettings;

        public SiteSignUpFormViewComponent(
            IOptions<PlatoSiteSettings> platoSiteSettings)
        {
            _platoSiteSettings = platoSiteSettings.Value;
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult((IViewComponentResult) View());
        }

    }

}
