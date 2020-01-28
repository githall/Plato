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

        private readonly PlatoGoogleOptions _platoGoogleOptions;

        public GoogleSchemeConfiguration(
            IOptions<PlatoGoogleOptions> platoGoogleOptions)
        {
            _platoGoogleOptions = platoGoogleOptions.Value;
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

            options.ClientId = _platoGoogleOptions.ClientId;
            options.ClientSecret = _platoGoogleOptions.ClientSecret;

            if (_platoGoogleOptions.CallbackPath.HasValue)
            {
                options.CallbackPath = _platoGoogleOptions.CallbackPath;
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

            if (string.IsNullOrEmpty(_platoGoogleOptions.ClientId)
                || string.IsNullOrEmpty(_platoGoogleOptions.ClientSecret))
            {
                return false;
            }

            return true;

        }

    }

}
