using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Plato.Users.Configuration
{

    public class CookieSchemeConfiguration :
        IConfigureOptions<AuthenticationOptions>
    {

        public void Configure(AuthenticationOptions options)
        {

            options.AddScheme(CookieAuthenticationDefaults.AuthenticationScheme, builder =>
            {
                builder.DisplayName = CookieAuthenticationDefaults.AuthenticationScheme;
                builder.HandlerType = typeof(CookieAuthenticationHandler);
            });

            options.AddScheme(IdentityConstants.ApplicationScheme, builder =>
            {
                builder.DisplayName = IdentityConstants.ApplicationScheme;                
                builder.HandlerType = typeof(CookieAuthenticationHandler);
            });

        }

    }

}
