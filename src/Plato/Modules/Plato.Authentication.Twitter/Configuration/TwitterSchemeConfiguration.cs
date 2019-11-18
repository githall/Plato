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

        private readonly TwitterAuthenticationOptions _twitterAuthenticationOptions;

        public TwitterSchemeConfiguration(
            IOptions<TwitterAuthenticationOptions> twitterAuthenticationOptions)
        {
            _twitterAuthenticationOptions = twitterAuthenticationOptions.Value;
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

            options.ConsumerKey = _twitterAuthenticationOptions.ConsumerKey;
            options.ConsumerSecret = _twitterAuthenticationOptions.ConsumerSecret;

            if (_twitterAuthenticationOptions.CallbackPath.HasValue)
            {
                options.CallbackPath = _twitterAuthenticationOptions.CallbackPath;
            }

        }

        bool ValidSettings()
        {

            if (string.IsNullOrEmpty(_twitterAuthenticationOptions.ConsumerKey)
                || string.IsNullOrEmpty(_twitterAuthenticationOptions.ConsumerSecret))
            {
                return false;
            }

            return true;

        }

    }

}
