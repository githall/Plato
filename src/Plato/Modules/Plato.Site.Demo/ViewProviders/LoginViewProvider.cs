using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Site.Demo.Models;
using Plato.Site.Demo.ViewModels;
using Plato.Users.ViewModels;

namespace Plato.Site.Demo.ViewProviders
{
    public class LoginViewProvider : BaseViewProvider<UserLogin>
    {
        
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        private readonly DemoOptions _demoOptions;

        public LoginViewProvider(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IOptions<DemoOptions> demoOptions)
        {     
            T = htmlLocalizer;
            S = stringLocalizer;
            _demoOptions = demoOptions.Value;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(
            UserLogin viewModel,
            IViewProviderContext context)
        {

            var loginViewModel = new DemoSettingsViewModel()
            {
                AdminUserName = _demoOptions.AdminUserName,
                AdminPassword = _demoOptions.AdminPassword
            };

            return Task.FromResult(Views(                
                View<DemoSettingsViewModel>("Demo.Login.Sidebar", model => loginViewModel).Zone("sidebar")                
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
