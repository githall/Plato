using Microsoft.AspNetCore.Identity;
using PlatoCore.Models.Users;

namespace PlatoCore.Security.Abstractions
{
    public interface IDummyClaimsPrincipalFactory<TUser> : IUserClaimsPrincipalFactory<TUser> where TUser : class, IUser
    {
    }

}
