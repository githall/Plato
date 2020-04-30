using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Layout.ModelBinding;
using Plato.Site.ViewModels;
using Plato.Site.Services;
using Plato.Site.Models;
using PlatoCore.Text.Abstractions;
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
                        ["id"] = result.Response.Id.ToString()
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

        public async Task<IActionResult> SignUpConfirmation(int id)
        {

            // Validate
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            // Get the sign-up
            var signUp = await _signUpStore.GetByIdAsync(id);

            // Ensure we found the sign-up
            if (signUp == null)
            {
                return NotFound();
            }

            return View(new SignUpConfirmationViewModel()
            {
                Id = signUp.Id,
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

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
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
            var signUp = await _signUpStore.GetByIdAsync(model.Id);

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
                    ["id"] = signUp.Id.ToString(),
                    ["token"] = signUp.SecurityToken
                });
            }

            // The confirmation code is incorrect
            ViewData.ModelState.AddModelError(string.Empty, "The confirmation code is incorrect. Please try again!");
            return await SignUpConfirmation(signUp.Id);

        }

        // ---------------------
        // 3. SetUp 
        // Ask for company name
        // ---------------------

        public async Task<IActionResult> SetUp(int id, string token)
        {

            // Validate
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            // Get the sign-up
            var signUp = await _signUpStore.GetByIdAsync(id);

            // Ensure we found the sign-up
            if (signUp == null)
            {
                return NotFound();
            }

            var validToken = signUp.SecurityToken.Equals(token, StringComparison.OrdinalIgnoreCase);
            if (validToken)
            {
                return NotFound();

            }

            return View(new SetUpViewModel()
            {
                Id = signUp.Id
            });

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(SignUpConfirmation))]
        public async Task<IActionResult> SetUpPost(SetUpViewModel model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get the sign-up
            var signUp = await _signUpStore.GetByIdAsync(model.Id);

            // Ensure we found the sign-up
            if (signUp == null)
            {
                return NotFound();
            }


            return View();


        }

    }

}
