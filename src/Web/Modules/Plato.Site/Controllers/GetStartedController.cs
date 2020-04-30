using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Layout.ModelBinding;
using Plato.Site.ViewModels;

namespace Plato.Site.Controllers
{
    public class GetStartedController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IEmailManager _emailManager;
        private readonly SmtpSettings _smtpSettings;

        public GetStartedController(
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

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        public Task<IActionResult> IndexPost(GetStartedViewModel model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                throw new ArgumentNullException(nameof(model.Email));
            }

            // Return view
            return Task.FromResult((IActionResult)View());

        }

        #endregion

    }

}
