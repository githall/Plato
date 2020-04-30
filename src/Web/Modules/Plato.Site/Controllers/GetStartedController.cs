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

        private readonly IKeyGenerator _keyGenerator;
        private readonly ISignUpManager<SignUp> _signUpManager;
        private readonly ISignUpStore<SignUp> _signUpStore;
        private readonly ISignUpEmails _signUpEmails;        

        public GetStartedController(
            ISignUpManager<SignUp> signUpManager,
            ISignUpStore<SignUp> signUpStore,
            IKeyGenerator keyGenerator,
            ISignUpEmails signUpEmails)
        {
            _signUpManager = signUpManager;            
            _signUpEmails = signUpEmails;
            _keyGenerator = keyGenerator;
            _signUpStore = signUpStore;
        }

        // ---------------------
        // Index 
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

            // Build simple security token
            var token = _keyGenerator.GenerateKey(o =>
            {
                o.OnlyDigits = true;
                o.MaxLength = 6;
            });

            // Create sign-up
            var result = await _signUpManager.CreateAsync(new SignUp()
            {
                Email = model.Email,
                SecurityToken = token
            });

            // Ensure sign-up was created successfully
            if (result.Succeeded)
            {

                // Send email
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

        public async Task<IActionResult> SignUpConfirmation(int id)
        {

            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var signUp = await _signUpStore.GetByIdAsync(id);

            return View();
        }


    }

}
