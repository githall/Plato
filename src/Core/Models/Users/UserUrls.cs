using Microsoft.AspNetCore.Routing;

namespace PlatoCore.Models.Users
{
    public class UserUrls
    {

        public RouteValueDictionary GetProfileRoute { get; }

        public UserUrls(ISimpleUser user)
        {      
            GetProfileRoute = new RouteValueDictionary()
            {
                ["area"] = "Plato.Users",
                ["controller"] = "Home",
                ["action"] = "GetUser",
                ["opts.id"] = user.Id,
                ["opts.alias"] = user.Alias
            };
        }

    }

}
