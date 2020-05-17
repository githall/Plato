﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Plato.Authentication.Google.ViewModels;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Users;


namespace Plato.Authentication.Google.ViewProviders
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

        public override Task<IViewProviderResult> BuildIndexAsync(
            LoginPage viewModel,
            IViewProviderContext context)
        {

            return Task.FromResult(Views(                
                View<GoogleLoginViewModel>("Google.Login.Sidebar", model => new GoogleLoginViewModel())
                    .Zone("content-right")
                    .Order(int.MinValue + 100)                
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

        public override async Task<IViewProviderResult> BuildUpdateAsync(LoginPage viewModel, IViewProviderContext context)
        {
            return await BuildIndexAsync(viewModel, context);
        }

        #endregion

    }

}
