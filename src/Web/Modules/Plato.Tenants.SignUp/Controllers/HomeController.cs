﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlatoCore.Layout.ModelBinding;
using Plato.Tenants.SignUp.ViewModels;
using Plato.Tenants.SignUp.Services;
using Microsoft.AspNetCore.Routing;
using Plato.Tenants.SignUp.Stores;
using Plato.Tenants.Services;
using Plato.Tenants.Models;
using PlatoCore.Emails.Abstractions;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using PlatoCore.Hosting.Web.Abstractions;
using Microsoft.AspNetCore.Http;
using PlatoCore.Models.Shell;
using PlatoCore.Models.Extensions;

namespace Plato.Tenants.SignUp.Controllers
{

    public class HomeController : Controller, IUpdateModel
    {

        private readonly ISignUpManager<Models.SignUp> _signUpManager;
        private readonly ISignUpStore<Models.SignUp> _signUpStore;
        private readonly ITenantSetUpService _tenantSetUpService;
        private readonly ISignUpEmailService _signUpEmails;
        private readonly IContextFacade _contextFacade;
        private readonly IShellSettings _shellSettings;

        private readonly DefaultTenantSettings _defaultTenantSettings;   

        public HomeController(            
            IOptions<DefaultTenantSettings> tenantSetings,
            ISignUpManager<Models.SignUp> signUpManager,
            ISignUpStore<Models.SignUp> signUpStore,
            ITenantSetUpService tenantSetUpService,
            ISignUpEmailService signUpEmails,
            IContextFacade contextFacade,
            IShellSettings shellSettings)
        {
            _defaultTenantSettings = tenantSetings.Value;                
            _tenantSetUpService = tenantSetUpService;            
            _signUpManager = signUpManager;
            _contextFacade = contextFacade;
            _shellSettings = shellSettings;
            _signUpEmails = signUpEmails;
            _signUpStore = signUpStore;            
        }

        // ---------------------
        // 1. SignUp
        // Ask for email, generate security token email
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Index()
        {

            // Ensure default shell, We cannot sign-up from tenants
            if (!_shellSettings.IsDefaultShell())
            {
                return Task.FromResult((IActionResult)Unauthorized());
            }

            // Return view
            return Task.FromResult((IActionResult)View());

        }

        [HttpPost, ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(SignUpViewModel model)
        {

            // Ensure default shell, We cannot sign-up from tenants
            if (!_shellSettings.IsDefaultShell())
            {
                return Unauthorized();
            }

            // We are intentionally not using cross site request forgery protection
            // for this POST request, we need to allow trusted 3rd party sites to post here
            // Instead ensure the request is referred from a trusted site            

            // Only allow posts from trusted domains    
            var allowedReferers = new List<string>()
            {                  
                "http://instantasp.co.uk",
                "https://instantasp.co.uk",
                "http://www.instantasp.co.uk",
                "https://www.instantasp.co.uk",
                "http://plato.instantasp.co.uk",
                "https://plato.instantasp.co.uk",
            };

            // Allow posts from our current host
            allowedReferers.Add(await _contextFacade.GetBaseUrlAsync());

            // Check referee, not perfect and to be improved
            var request = HttpContext.Request;
            var header = request.GetTypedHeaders();
            var referer = header.Referer?.ToString() ?? string.Empty;
            var allowReferer = false;
            foreach (var allowedReferer in allowedReferers)
            {
                if (referer.StartsWith(allowedReferer, StringComparison.OrdinalIgnoreCase))
                {
                    allowReferer = true;
                    break;
                }
            }

            // The referee is not allowed
            if (!allowReferer)
            {
                return Unauthorized();
            }

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
            var result = await _signUpManager.CreateAsync(new Models.SignUp()
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
                    return RedirectToAction(nameof(IndexConfirmation), new RouteValueDictionary()
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
        public async Task<IActionResult> IndexConfirmation(string sessionId)
        {

            // Ensure default shell, We cannot sign-up from tenants
            if (!_shellSettings.IsDefaultShell())
            {
                return Unauthorized();
            }

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

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(IndexConfirmation))]
        public async Task<IActionResult> IndexConfirmationPost(SignUpConfirmationViewModel model)
        {

            // Ensure default shell, We cannot sign-up from tenants
            if (!_shellSettings.IsDefaultShell())
            {
                return Unauthorized();
            }

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
            return await IndexConfirmation(signUp.SessionId);

        }

        // ---------------------
        // 3. SetUp 
        // Ask for company name
        // ---------------------

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> SetUp(string sessionId)
        {

            // Ensure default shell, We cannot sign-up from tenants
            if (!_shellSettings.IsDefaultShell())
            {
                return Unauthorized();
            }

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

            // Ensure default shell, We cannot sign-up from tenants
            if (!_shellSettings.IsDefaultShell())
            {
                return Unauthorized();
            }

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
        // Ask for administrator user name & password
        // ---------------------

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> SetUpConfirmation(string sessionId)
        {

            // Ensure default shell, We cannot sign-up from tenants
            if (!_shellSettings.IsDefaultShell())
            {
                return Unauthorized();
            }

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

            // Ensure default shell, We cannot sign-up from tenants
            if (!_shellSettings.IsDefaultShell())
            {
                return Unauthorized();
            }

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

            // Set sign-up email address as default sender for tenant emails
            if (_defaultTenantSettings.SmtpSettings != null)
            {
                _defaultTenantSettings.SmtpSettings.DefaultFrom = signUp.Email;
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

        // ---------------------
        // 5. Setup Complete
        // ---------------------

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> SetUpComplete(string sessionId)
        {

            // Ensure default shell, We cannot sign-up from tenants
            if (!_shellSettings.IsDefaultShell())
            {
                return Unauthorized();
            }

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
            var hostUrl = await _contextFacade.GetBaseUrlAsync();
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
