using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.Extensions.Options;
using Plato.Twitter.Models;

namespace Plato.Authentication.Twitter.Configuration
{

    public class TwitterSchemeConfiguration :
        IConfigureOptions<AuthenticationOptions>,
        IConfigureNamedOptions<TwitterOptions>
    {

        private readonly PlatoTwitterOptions _platoTwitterOptions;

        public TwitterSchemeConfiguration(
            IOptions<PlatoTwitterOptions> platoTwitterOptions)
        {
            _platoTwitterOptions = platoTwitterOptions.Value;
        }

        public void Configure(AuthenticationOptions options)
        {

            if (!ValidSettings())
            {
                return;
            }

            options.AddScheme(TwitterDefaults.AuthenticationScheme, builder =>
            {
                builder.DisplayName = "Twitter";
                builder.HandlerType = typeof(TwitterHandler);                
            });

        }

        public void Configure(TwitterOptions options) =>
            Configure(TwitterDefaults.AuthenticationScheme, options);

        public void Configure(string name, TwitterOptions options)
        {

            if (!string.Equals(name, TwitterDefaults.AuthenticationScheme, StringComparison.Ordinal))
            {
                return;
            }

            if (!ValidSettings())
            {
                return;
            }

            options.ConsumerKey = _platoTwitterOptions.ConsumerKey;
            options.ConsumerSecret = _platoTwitterOptions.ConsumerSecret;

            if (_platoTwitterOptions.CallbackPath.HasValue)
            {
                options.CallbackPath = _platoTwitterOptions.CallbackPath;
            }

        }

        bool ValidSettings()
        {

            if (string.IsNullOrEmpty(_platoTwitterOptions.ConsumerKey)
                || string.IsNullOrEmpty(_platoTwitterOptions.ConsumerSecret))
            {
                return false;
            }

            return true;

        }

    }

}
