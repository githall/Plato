using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using Plato.Google.Models;

namespace Plato.Authentication.Google.Configuration
{

    public class GoogleSchemaConfiguration :
        IConfigureOptions<AuthenticationOptions>,
        IConfigureNamedOptions<GoogleOptions>
    {

        private readonly GoogleAuthenticationOptions _googleOptions;

        public GoogleSchemaConfiguration(
            IOptions<GoogleAuthenticationOptions> googleOptions)
        {
            _googleOptions = googleOptions.Value;
        }

        public void Configure(AuthenticationOptions options)
        {

            if (string.IsNullOrEmpty(_googleOptions.ClientId))
            {
                return;
            }

            if (string.IsNullOrEmpty(_googleOptions.ClientSecret))
            {
                return;
            }

            options.AddScheme(GoogleDefaults.AuthenticationScheme, builder =>
            {
                builder.DisplayName = "Google";
                builder.HandlerType = typeof(GoogleHandler);
            });

        }

        public void Configure(GoogleOptions options)
        {
            options.ClientId = _googleOptions.ClientId;
            options.ClientSecret = _googleOptions.ClientSecret;
        }

        public void Configure(string name, GoogleOptions options)
        {

            if (!string.Equals(name, GoogleDefaults.AuthenticationScheme, StringComparison.Ordinal))
            {
                return;
            }

            options.ClientId = _googleOptions.ClientId;
            options.ClientSecret = _googleOptions.ClientSecret;

        }

    }

}
