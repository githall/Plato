using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PlatoCore.Models.Users;

namespace PlatoCore.Security
{

    /// <summary>
    // A custom UserClaimsPrincipalFactory implementation.
    // Roles within Plato can contain many claims. For this reason to avoid cookie
    // chunking and exceeding maximum request header length issues caused by persisting role claims
    // within a cookie we don't persist the role claims within the cookie and instead
    // query these claims as necessary based on our minimal claims principal created by UserClaimsPrincipalFactory<TUser>
    // Ref: https://github.com/aspnet/Identity/blob/master/src/Core/UserClaimsPrincipalFactory.cs
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TRole"></typeparam>
    public class PlatoUserClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser>
        where TUser : class, IUser
        where TRole : class
    {
        public PlatoUserClaimsPrincipalFactory(
             UserManager<TUser> userManager,
             IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }

    }

}