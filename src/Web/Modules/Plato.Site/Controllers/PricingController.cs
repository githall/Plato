using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Layout.ModelBinding;

namespace Plato.Site.Controllers
{
    public class PricingController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IEmailManager _emailManager;
        private readonly SmtpSettings _smtpSettings;


        public PricingController(
            IEmailManager emailManager,
            IOptions<SmtpSettings> smtpSettings)
        {
            _emailManager = emailManager;
            _smtpSettings = smtpSettings.Value;
        }

        #endregion

        #region "Actions"

        // ---------------------
        // Index
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Index()
        {
            // Return view
            return Task.FromResult((IActionResult) View());

        }

        #endregion

    }

}
