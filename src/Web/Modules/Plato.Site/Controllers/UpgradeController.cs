using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlatoCore.Layout.ModelBinding;

namespace Plato.Site.Controllers
{
    public class UpgradeController : Controller, IUpdateModel
    {

        // ---------------------
        // Index
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Index()
        {
            // Return view
            return Task.FromResult((IActionResult)View());

        }

    }

}
