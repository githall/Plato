using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.Extensions.Options;
using Plato.Facebook.Models;

namespace Plato.Authentication.Facebook.Configuration
{

    public class FacebookSchemeConfiguration :
        IConfigureOptions<AuthenticationOptions>,
        IConfigureNamedOptions<FacebookOptions>
    {

        private readonly FacebookAuthenticationOptions _facebookAuthenticationOptions;

        public FacebookSchemeConfiguration(
            IOptions<FacebookAuthenticationOptions> facebookAuthenticationOptions)
        {
            _facebookAuthenticationOptions = facebookAuthenticationOptions.Value;
        }

        public void Configure(AuthenticationOptions options)
        {

            if (!ValidSettings())
            {
                return;
            }

            options.AddScheme(FacebookDefaults.AuthenticationScheme, builder =>
            {
                builder.DisplayName = "Facebook";
                builder.HandlerType = typeof(FacebookHandler);                
            });

        }

        public void Configure(FacebookOptions options) =>
            Configure(FacebookDefaults.AuthenticationScheme, options);

        public void Configure(string name, FacebookOptions options)
        {

            if (!string.Equals(name, FacebookDefaults.AuthenticationScheme, StringComparison.Ordinal))
            {
                return;
            }

            if (!ValidSettings())
            {
                return;
            }

            options.AppId = _facebookAuthenticationOptions.AppId;
            options.AppSecret = _facebookAuthenticationOptions.AppSecret;

        }

        bool ValidSettings()
        {

            if (string.IsNullOrEmpty(_facebookAuthenticationOptions.AppId)
                || string.IsNullOrEmpty(_facebookAuthenticationOptions.AppSecret))
            {
                return false;
            }

            return true;

        }

    }

}
