using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using Plato.Google.Models;

namespace Plato.Authentication.Google.Configuration
{

    public class GoogleSchemeConfiguration :
        IConfigureOptions<AuthenticationOptions>,
        IConfigureNamedOptions<GoogleOptions>
    {

        private readonly GoogleAuthenticationOptions _googleAuthenticationOptions;

        public GoogleSchemeConfiguration(
            IOptions<GoogleAuthenticationOptions> googleOptions)
        {
            _googleAuthenticationOptions = googleOptions.Value;
        }

        public void Configure(AuthenticationOptions options)
        {
            
            if (!ValidSettings())
            {
                return;
            }

            options.AddScheme(GoogleDefaults.AuthenticationScheme, builder =>
            {
                builder.DisplayName = "Google";
                builder.HandlerType = typeof(GoogleHandler);                
            });

        }

        public void Configure(GoogleOptions options) =>
            Configure(GoogleDefaults.AuthenticationScheme, options);

        public void Configure(string name, GoogleOptions options)
        {

            if (!string.Equals(name, GoogleDefaults.AuthenticationScheme, StringComparison.Ordinal))
            {
                return;
            }

            if (!ValidSettings())
            {
                return;
            }

            options.ClientId = _googleAuthenticationOptions.ClientId;
            options.ClientSecret = _googleAuthenticationOptions.ClientSecret;

            if (_googleAuthenticationOptions.CallbackPath.HasValue)
            {
                options.CallbackPath = _googleAuthenticationOptions.CallbackPath;
            }

            options.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
            options.ClaimActions.Clear();
            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
            options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
            options.ClaimActions.MapJsonKey("urn:google:profile", "link");
            options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

        }

        bool ValidSettings()
        {

            if (string.IsNullOrEmpty(_googleAuthenticationOptions.ClientId)
                || string.IsNullOrEmpty(_googleAuthenticationOptions.ClientSecret))
            {
                return false;
            }

            return true;

        }

    }

}
