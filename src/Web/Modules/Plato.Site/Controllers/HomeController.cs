using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Layout.ModelBinding;
using Plato.Site.ViewModels;

namespace Plato.Site.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IEmailManager _emailManager;
        private readonly SmtpSettings _smtpSettings;


        public HomeController(
            IEmailManager emailManager,
            IOptions<SmtpSettings> smtpSettings)
        {
            _emailManager = emailManager;
            _smtpSettings = smtpSettings.Value;
        }

        #endregion

        #region "Actions"

        // ---------------------
        // Index / Home
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Index()
        {
            // Return view
            return Task.FromResult((IActionResult) View());

        }

        // ---------------------
        // Support Options
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> SupportOptions()
        {
            // Return view
            return Task.FromResult((IActionResult)View());

        }

        #endregion

    }

}
