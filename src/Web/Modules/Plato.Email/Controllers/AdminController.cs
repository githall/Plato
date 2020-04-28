﻿using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Layout;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Email.ViewModels;

namespace Plato.Email.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<EmailSettings> _viewProvider;
        private readonly IAuthorizationService _authorizationService;                
        private readonly IBreadCrumbManager _breadCrumbManager;        
        private readonly IEmailManager _emailManager;
        private readonly IAlerter _alerter;

        private readonly SmtpSettings _smtpSettings;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IViewProviderManager<EmailSettings> viewProvider,
            IAuthorizationService authorizationService,
            IBreadCrumbManager breadCrumbManager,
            IOptions<SmtpSettings> smtpSettings,
            IEmailManager emailManager,
            IAlerter alerter)
        {

            _authorizationService = authorizationService;
            _breadCrumbManager = breadCrumbManager;
            _viewProvider = viewProvider;
            _smtpSettings = smtpSettings.Value;
            _emailManager = emailManager;

            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }
        
        public async Task<IActionResult> Index()
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageEmailSettings))
            {
                return Unauthorized();
            }
            
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Settings"], channels => channels
                    .Action("Index", "Admin", "Plato.Settings")
                    .LocalNav()
                ).Add(S["Email"]);
            });

            // Return view
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(new EmailSettings(), this));
            
        }
        
        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(EmailSettingsViewModel viewModel)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageEmailSettings))
            {
                return Unauthorized();
            }

            // Execute view providers ProvideUpdateAsync method
            await _viewProvider.ProvideUpdateAsync(new EmailSettings(), this);
        
            // Add alert
            _alerter.Success(T["Settings Updated Successfully!"]);
      
            return RedirectToAction(nameof(Index));
            
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SendTest()
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageEmailSettings))
            {
                return Unauthorized();
            }

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Settings"], settings => settings
                    .Action("Index", "Admin", "Plato.Settings")
                    .LocalNav()
                ).Add(S["Email"], email => email
                    .Action("Index", "Admin", "Plato.Email")
                    .LocalNav()
                ).Add(S["Test Email"]);
            });
            
            // We always need a default sender to send the test to and from
            if (string.IsNullOrEmpty(_smtpSettings.DefaultFrom))
            {           
                ViewData.ModelState.AddModelError(string.Empty, "You must specify a default sender first.");
                return await Index();
            }

            // Build message
            var message = new MailMessage(
                new MailAddress(_smtpSettings.DefaultFrom), 
                new MailAddress(_smtpSettings.DefaultFrom))
            {
                Subject = "Test email from Plato",
                Body = WebUtility.HtmlDecode(GetTestMessage())
            };
          
            // Send test message
            var result = await _emailManager.SendAsync(message);

            // Success?
            if (result.Succeeded)
            {
                _alerter.Success(T["Email Sent Successfully!"]);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ViewData.ModelState.AddModelError(error.Code, $"{error.Code} - {error.Description}");
                }
            }
          
            return await Index();

        }

        string GetTestMessage()
        {

            return @"Hi There,

Congratulations outbound emails within Plato are configured and working correctly.

You are receiving this email because you or someone used the test email button on the Plato email settings page. 

Thank you for using Plato.";

        }

    }
    
}
