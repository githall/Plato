using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using Plato.Google.Models;
using System;
using System.Collections.Generic;
using System.Text;

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

            options.AddScheme(GoogleDefaults.AuthenticationScheme, builder =>
            {
                builder.DisplayName = "Google";
                builder.HandlerType = typeof(GoogleHandler);
            });


        }

        public void Configure(GoogleOptions options)
        {
            options.ClientId = _googleOptions.AppId;
            options.ClientSecret = _googleOptions.AppSecret;

        }

        public void Configure(string name, GoogleOptions options)
        {
            throw new NotImplementedException();
        }

    }
}
