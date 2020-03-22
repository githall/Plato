using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Layout.ModelBinding;

namespace Plato.Site.Controllers
{
    public class LegalController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IEmailManager _emailManager;
        private readonly SmtpSettings _smtpSettings;


        public LegalController(
            IEmailManager emailManager,
            IOptions<SmtpSettings> smtpSettings)
        {
            _emailManager = emailManager;
            _smtpSettings = smtpSettings.Value;
        }

        #endregion

        #region "Actions"

        // ---------------------
        // License
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> License()
        {
            // Return view
            return Task.FromResult((IActionResult)View());

        }

        // ---------------------
        // Terms
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Terms()
        {
            // Return view
            return Task.FromResult((IActionResult) View());

        }

        // ---------------------
        // Privacy
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Privacy()
        {
            // Return view
            return Task.FromResult((IActionResult)View());

        }

        #endregion

    }

}
