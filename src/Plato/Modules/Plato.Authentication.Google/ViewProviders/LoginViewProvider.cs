using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Plato.Authentication.Google.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;


namespace Plato.Authentication.Google.ViewProviders
{
    public class LoginViewProvider : BaseViewProvider<UserLogin>
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
            UserLogin viewModel,
            IViewProviderContext context)
        {

            return Task.FromResult(Views(                
                View<GoogleLoginViewModel>("Google.Login.Sidebar", model => new GoogleLoginViewModel())
                    .Zone("sidebar")
                    .Order(int.MinValue + 100)                
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(UserLogin viewModel,
            IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(UserLogin viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(UserLogin viewModel, IViewProviderContext context)
        {
            return await BuildIndexAsync(viewModel, context);
        }

        #endregion

    }

}
