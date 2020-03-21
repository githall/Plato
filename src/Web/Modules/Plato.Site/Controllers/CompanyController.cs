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
    public class CompanyController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IEmailManager _emailManager;
        private readonly SmtpSettings _smtpSettings;


        public CompanyController(
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

        // ---------------------
        // Contact
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Contact()
        {
            // Return view
            return Task.FromResult((IActionResult) View());

        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactFormViewModel viewModel)
        {

            var from = viewModel.Email;
            var body = !string.IsNullOrEmpty(viewModel.Message)
                ? WebUtility.HtmlDecode(viewModel.Message)
                : viewModel.Message;

            if (string.IsNullOrEmpty(from))
            {
                ViewData.ModelState.AddModelError(string.Empty, "The \"from\" field is required!");
                return await Contact(viewModel);
            }

            if (string.IsNullOrEmpty(body))
            {
                ViewData.ModelState.AddModelError(string.Empty, "The \"message\" field is required!");
                return await Contact(viewModel);
            }

            // Build message
            var message = new MailMessage
            {
                From = new MailAddress(from.Trim()),
                Subject = "Plato Feedback Form",
                Body  = body,
                IsBodyHtml = true,
            };

            message.To.Add(_smtpSettings.DefaultFrom);

            // Send message
            var result = await _emailManager.SaveAsync(message);
            if (result.Succeeded)
            {
                // Success - Redirect to confirmation page
                return RedirectToAction(nameof(ContactConfirmation));
            }

            return await Contact(viewModel);

        }

        [HttpGet, AllowAnonymous]
        public IActionResult ContactConfirmation()
        {
            // Return view
            return View();

        }

        #endregion

    }

}
