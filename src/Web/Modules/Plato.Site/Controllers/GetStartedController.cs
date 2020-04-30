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

namespace Plato.Site.Controllers
{

    public class GetStartedController : Controller, IUpdateModel
    {


        private readonly ISignUpManager<SignUp> _signUpManager;
        private readonly ISignUpStore<SignUp> _signUpStore;
        private readonly ISignUpEmails _signUpEmails;        

        public GetStartedController(
            ISignUpManager<SignUp> signUpManager,
            ISignUpStore<SignUp> signUpStore,          
            ISignUpEmails signUpEmails)
        {
            _signUpManager = signUpManager;            
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

            // Return view
            return Task.FromResult((IActionResult) View());

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

            signUp.CompanyName = model.CompanyName;

            var result = await _signUpManager.UpdateAsync(signUp);

            if (result.Succeeded)
            {

                // --------------------
                // Create tenant here
                // --------------------


                // Redirect to sign-up confirmation
                return RedirectToAction(nameof(SetUpConfirmation), new RouteValueDictionary()
                {
                    ["sessionId"] = signUp.SessionId
                });
            }

            // The company name may be invalid or some other issues occurred
            ViewData.ModelState.AddModelError(string.Empty, "The confirmation code is incorrect. Please try again!");
            return await SignUpConfirmation(signUp.SessionId);

        }
     
        // ---------------------
        // 4. SetUp Confirmation
        // Link to Plato installation
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

            return View(new SetUpConfirmationViewModel()
            {
            });

        }

    }

}
