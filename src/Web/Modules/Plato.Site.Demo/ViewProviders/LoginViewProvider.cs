using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Users;
using Plato.Site.Demo.Models;
using Plato.Site.Demo.ViewModels;
using Plato.Users.ViewModels;

namespace Plato.Site.Demo.ViewProviders
{
    public class LoginViewProvider : ViewProviderBase<LoginPage>
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
            LoginPage viewModel,
            IViewProviderContext context)
        {

            var loginViewModel = new DemoSettingsViewModel()
            {
                AdminUserName = _demoOptions.AdminUserName,
                AdminPassword = _demoOptions.AdminPassword
            };

            return Task.FromResult(Views(                
                View<DemoSettingsViewModel>("Demo.Login.Sidebar", model => loginViewModel).Zone("content-right")                
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
