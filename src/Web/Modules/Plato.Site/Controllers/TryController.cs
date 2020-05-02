using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlatoCore.Layout.ModelBinding;
using Plato.Site.ViewModels;
using Plato.Site.Services;
using Plato.Site.Models;
using Microsoft.AspNetCore.Routing;
using Plato.Site.Stores;
using Plato.Tenants.Services;
using Plato.Tenants.Models;
using PlatoCore.Emails.Abstractions;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using PlatoCore.Hosting.Web.Abstractions;

namespace Plato.Site.Controllers
{

    public class TryController : Controller, IUpdateModel
    {

        private readonly ITenantSetUpService _tenantSetUpService;
        private readonly ISignUpManager<SignUp> _signUpManager;
        private readonly ISignUpStore<SignUp> _signUpStore;
        private readonly ISignUpEmailService _signUpEmails;
        private readonly IContextFacade _contextFacade;

        private readonly DefaultTenantSettings _defaultTenantSettings;
        private readonly PlatoSiteSettings _platoSiteSettings;

        public TryController(
            IOptions<DefaultTenantSettings> tenantSetings,
            IOptions<PlatoSiteSettings> platoSiteSettings,
            ITenantSetUpService tenantSetUpService,
            ISignUpManager<SignUp> signUpManager,
            ISignUpStore<SignUp> signUpStore,
            ISignUpEmailService signUpEmails,
            IContextFacade contextFacade)
        {
            _defaultTenantSettings = tenantSetings.Value;
            _platoSiteSettings = platoSiteSettings.Value;
            _tenantSetUpService = tenantSetUpService;
            _signUpManager = signUpManager;
            _contextFacade = contextFacade;
            _signUpEmails = signUpEmails;
            _signUpStore = signUpStore;
        }

        // ---------------------
        // 1. SignUp
        // Ask for email, generate security token email
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> SignUp()
        {

            // Redirect to tenant host if one is provided
            if (!string.IsNullOrEmpty(_platoSiteSettings.HostUrl))
            {
                return Task.FromResult((IActionResult)Redirect(_platoSiteSettings.HostUrl));
            }

            // Return view
            return Task.FromResult((IActionResult)View());

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(SignUp))]
        public async Task<IActionResult> SignUpPost(SignUpViewModel model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                throw new ArgumentNullException(nameof(model.Email));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Create sign-up
            var result = await _signUpManager.CreateAsync(new SignUp()
            {
                Email = model.Email,
                EmailUpdates = model.EmailUpdates
            });

            // Ensure sign-up was created successfully
            if (result.Succeeded)
            {

                // Send security token email
                var emailResult = await _signUpEmails.SendSecurityTokenAsync(result.Response);

                // Ensure email was sent successfully
                if (emailResult.Succeeded)
                {
                    // Redirect to sign-up confirmation
                    return RedirectToAction(nameof(SignUpConfirmation), new RouteValueDictionary()
                    {
                        ["sessionId"] = result.Response.SessionId.ToString()
                    });
                }
                else
                {
                    foreach (var error in emailResult.Errors)
                    {
                        ViewData.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ViewData.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we reach this point errors occurred
            return View(model);

        }

        // ---------------------
        // 2. SignUp Confirmation 
        // Enter security token to confirm email
        // ---------------------

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> SignUpConfirmation(string sessionId)
        {

            // Validate
            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentNullException(nameof(sessionId));
            }

            // Get the sign-up
            var signUp = await _signUpStore.GetBySessionIdAsync(sessionId);

            // Ensure we found the sign-up
            if (signUp == null)
            {
                return NotFound();
            }

            return View(new SignUpConfirmationViewModel()
            {
                SessionId = signUp.SessionId,
                Email = signUp.Email
            });

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(SignUpConfirmation))]
        public async Task<IActionResult> SignUpConfirmationPost(SignUpConfirmationViewModel model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.SessionId))
            {
                throw new ArgumentNullException(nameof(model.SessionId));
            }

            if (string.IsNullOrEmpty(model.SecurityToken))
            {
                throw new ArgumentNullException(nameof(model.SecurityToken));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get the sign-up
            var signUp = await _signUpStore.GetBySessionIdAsync(model.SessionId);

            // Ensure we found the sign-up
            if (signUp == null)
            {
                return NotFound();
            }

            // Validate token
            var validToken = signUp.SecurityToken.Equals(model.SecurityToken, StringComparison.OrdinalIgnoreCase);
            if (validToken)
            {

                // Reset security token & mark email as confirmed 
                signUp.SecurityToken = string.Empty;
                signUp.EmailConfirmed = true;
                await _signUpStore.UpdateAsync(signUp);

                // Redirect to sign-up confirmation
                return RedirectToAction(nameof(SetUp), new RouteValueDictionary()
                {
                    ["sessionId"] = signUp.SessionId
                });

            }

            // The confirmation code is incorrect
            ViewData.ModelState.AddModelError(string.Empty, "The confirmation code is incorrect. Please try again!");
            return await SignUpConfirmation(signUp.SessionId);

        }

        // ---------------------
        // 3. SetUp 
        // Ask for company name
        // ---------------------

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> SetUp(string sessionId)
        {

            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentNullException(nameof(sessionId));
            }

            // Get the sign-up
            var signUp = await _signUpStore.GetBySessionIdAsync(sessionId);

            // Ensure we found the sign-up
            if (signUp == null)
            {
                return NotFound();
            }

            // Email must be confirmed
            if (!signUp.EmailConfirmed)
            {
                return Unauthorized();
            }

            // Return view
            return View(new SetUpViewModel()
            {
                SessionId = signUp.SessionId
            });

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(SetUp))]
        public async Task<IActionResult> SetUpPost(SetUpViewModel model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.SessionId))
            {
                throw new ArgumentOutOfRangeException(nameof(model.SessionId));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get the sign-up
            var signUp = await _signUpStore.GetBySessionIdAsync(model.SessionId);

            // Ensure we found the sign-up
            if (signUp == null)
            {
                return NotFound();
            }

            // Email must be confirmed
            if (!signUp.EmailConfirmed)
            {
                return Unauthorized();
            }

            // Add company name
            signUp.CompanyName = model.CompanyName;

            // Persist 
            var result = await _signUpManager.UpdateAsync(signUp);

            // Success?
            if (result.Succeeded)
            {
                // Redirect to sign-up confirmation
                return RedirectToAction(nameof(SetUpConfirmation), new RouteValueDictionary()
                {
                    ["sessionId"] = signUp.SessionId
                });
            }

            // The company name may be invalid or some other issues occurred
            foreach (var error in result.Errors)
            {
                ViewData.ModelState.AddModelError(error.Code, error.Description);
            }

            return await SetUp(signUp.SessionId);

        }

        // ---------------------
        // 4. SetUp Confirmation
        // Ask for administrator username & password
        // ---------------------

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> SetUpConfirmation(string sessionId)
        {

            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentOutOfRangeException(nameof(sessionId));
            }

            // Get the sign-up
            var signUp = await _signUpStore.GetBySessionIdAsync(sessionId);

            // Ensure we found the sign-up
            if (signUp == null)
            {
                return NotFound();
            }

            // Email must be confirmed
            if (!signUp.EmailConfirmed)
            {
                return Unauthorized();
            }

            return View(new SetUpConfirmationViewModel()
            {
                SessionId = sessionId
            });

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(SetUpConfirmation))]
        public async Task<IActionResult> SetUpConffirmationPost(SetUpConfirmationViewModel model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.SessionId))
            {
                throw new ArgumentOutOfRangeException(nameof(model.SessionId));
            }

            if (string.IsNullOrEmpty(model.UserName))
            {
                throw new ArgumentOutOfRangeException(nameof(model.UserName));
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                throw new ArgumentOutOfRangeException(nameof(model.Password));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get the sign-up
            var signUp = await _signUpStore.GetBySessionIdAsync(model.SessionId);

            // Ensure we found the sign-up
            if (signUp == null)
            {
                return NotFound();
            }

            // Email must be confirmed
            if (!signUp.EmailConfirmed)
            {
                return Unauthorized();
            }

            // Create tenant context
            var setupContext = new TenantSetUpContext()
            {
                SiteName = signUp.CompanyName,
                Location = $"tenant{signUp.Id.ToString()}",
                DatabaseProvider = "SqlClient",
                DatabaseConnectionString = _defaultTenantSettings.ConnectionString,
                DatabaseTablePrefix = $"tenant{signUp.Id.ToString()}",
                AdminUsername = model.UserName,
                AdminEmail = signUp.Email,
                AdminPassword = model.Password,
                RequestedUrlPrefix = signUp.CompanyNameAlias,
                OwnerId = signUp.Email,
                CreatedDate = DateTimeOffset.Now,
                EmailSettings = new EmailSettings()
                {
                    SmtpSettings = _defaultTenantSettings.SmtpSettings
                },
                Errors = new Dictionary<string, string>()
            };

            // Attempt to create tenant
            var result = await _tenantSetUpService.InstallAsync(setupContext);

            // Report any errors
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ViewData.ModelState.AddModelError(error.Code, error.Description);
                }
                return await SetUp(signUp.SessionId);

            }

            // Redirect to set-up complete
            return RedirectToAction(nameof(SetUpComplete), new RouteValueDictionary()
            {
                ["sessionId"] = signUp.SessionId
            });

        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> SetUpComplete(string sessionId)
        {

            // Validate
            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentNullException(nameof(sessionId));
            }

            // Get the sign-up
            var signUp = await _signUpStore.GetBySessionIdAsync(sessionId);

            // Ensure we found the sign-up
            if (signUp == null)
            {
                return NotFound();
            }

            // Email must be confirmed
            if (!signUp.EmailConfirmed)
            {
                return Unauthorized();
            }

            // Get tenant host URL
            var hostUrl = _platoSiteSettings.HostUrl;

            if (string.IsNullOrEmpty(hostUrl))
            {
                hostUrl = await _contextFacade.GetBaseUrlAsync();
            }

            if (!string.IsNullOrEmpty(hostUrl))
            {
                if (hostUrl.EndsWith("/"))
                {
                    hostUrl = hostUrl.Substring(hostUrl.Length - 1);
                }
            }

            // Return view
            return View(new SetUpCompleteViewModel()
            {
                SessionId = signUp.SessionId,
                Url = $"{hostUrl}/{signUp.CompanyNameAlias}"
            });

        }
        
    }

}
