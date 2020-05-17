﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class LoginViewProvider : ViewProviderBase<LoginPage>
    {
        
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public LoginViewProvider(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer)
        {
     
            T = htmlLocalizer;
            S = stringLocalizer;

        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(LoginPage viewModel,
            IViewProviderContext context)
        {

            var loginViewModel = new LoginViewModel()
            {
                UserName = viewModel.UserName,
                Password = viewModel.Password
            };

            return Task.FromResult(Views(
                View<LoginViewModel>("Login.Index.Header", model => loginViewModel).Zone("header"),
                View<LoginViewModel>("Login.Index.Content", model => loginViewModel).Zone("content"),
                View<LoginViewModel>("Login.Index.Sidebar", model => loginViewModel).Zone("content-right"),
                View<LoginViewModel>("Login.Index.Footer", model => loginViewModel).Zone("actions")
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(LoginPage viewModel,
            IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(LoginPage viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<bool> ValidateModelAsync(LoginPage userLogin, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new LoginViewModel
            {
                UserName = userLogin.UserName,
                Password = userLogin.Password,
                RememberMe = userLogin.RememberMe
            });
        }

        public override async Task ComposeModelAsync(LoginPage userLogin, IUpdateModel updater)
        {

            var model = new LoginViewModel()
            {
                UserName = userLogin.UserName,
                Password = userLogin.Password,
                RememberMe = userLogin.RememberMe
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {
                userLogin.UserName = model.UserName;
                userLogin.Password = model.Password;
                userLogin.RememberMe = model.RememberMe;
            }

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(LoginPage viewModel, IViewProviderContext context)
        {
            return await BuildIndexAsync(viewModel, context);
        }

        #endregion

    }

}
