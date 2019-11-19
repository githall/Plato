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

        private readonly FacebookAuthenticationOptions _platoFacebookOptions;

        public FacebookSchemeConfiguration(
            IOptions<FacebookAuthenticationOptions> platoFacebookOptions)
        {
            _platoFacebookOptions = platoFacebookOptions.Value;
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

            options.AppId = _platoFacebookOptions.AppId;
            options.AppSecret = _platoFacebookOptions.AppSecret;

            if (_platoFacebookOptions.CallbackPath.HasValue)
            {
                options.CallbackPath = _platoFacebookOptions.CallbackPath;
            }

        }

        bool ValidSettings()
        {

            if (string.IsNullOrEmpty(_platoFacebookOptions.AppId)
                || string.IsNullOrEmpty(_platoFacebookOptions.AppSecret))
            {
                return false;
            }

            return true;

        }

    }

}
