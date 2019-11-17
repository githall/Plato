using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;

using Microsoft.Extensions.Options;
using Plato.Facebook.Models;

namespace Plato.Authentication.Facebook.Configuration
{

    public class FacebookSchemaConfiguration :
        IConfigureOptions<AuthenticationOptions>,
        IConfigureNamedOptions<FacebookOptions>
    {

        private readonly FacebookAuthenticationOptions _googleAuthenticationOptions;

        public FacebookSchemaConfiguration(
            IOptions<FacebookAuthenticationOptions> googleOptions)
        {
            _googleAuthenticationOptions = googleOptions.Value;
        }

        public void Configure(AuthenticationOptions options)
        {

            if (string.IsNullOrEmpty(_googleAuthenticationOptions.AppId))
            {
                return;
            }

            if (string.IsNullOrEmpty(_googleAuthenticationOptions.AppSecret))
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

            if (string.IsNullOrEmpty(_googleAuthenticationOptions.AppId))
            {
                return;
            }

            if (string.IsNullOrEmpty(_googleAuthenticationOptions.AppSecret))
            {
                return;
            }

            options.ClientId = _googleAuthenticationOptions.AppId;
            options.ClientSecret = _googleAuthenticationOptions.AppSecret;

            //if (_googleAuthenticationOptions.CallbackPath.HasValue)
            //{
            //    options.CallbackPath = _googleAuthenticationOptions.CallbackPath;
            //}

            options.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
            options.ClaimActions.Clear();
            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
            options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
            options.ClaimActions.MapJsonKey("urn:google:profile", "link");
            options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

            //// Optional
            //options.Events = new OAuthEvents
            //{   
            //    OnCreatingTicket = OnCreatingGoogleTicket()
            //}; // Event to capture when the authentication ticket is being created

        }

        //private static Func<OAuthCreatingTicketContext, Task> OnCreatingGoogleTicket()
        //{
        //    return async context =>
        //    {
        //        var firstName = context.Identity.FindFirst(ClaimTypes.GivenName).Value;
        //        var lastName = context.Identity.FindFirst(ClaimTypes.Surname)?.Value;
        //        var email = context.Identity.FindFirst(ClaimTypes.Email).Value;

        //        //Todo: Add logic here to save info into database

        //        // this Task.FromResult is purely to make the code compile as it requires a Task result
        //        await Task.FromResult(true);
        //    };
        //}

    }

}
