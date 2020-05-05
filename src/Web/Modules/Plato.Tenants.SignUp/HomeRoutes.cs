using System.Collections.Generic;
using PlatoCore.Abstractions.Routing;

namespace Plato.Tenants.SignUp
{

    public class HomeRoutes : IHomeRouteProvider
    {
        public IEnumerable<HomeRoute> GetRoutes()
        {
            return new[]
            {
                new HomeRoute("Plato.Tenants.SignUp", "Home", "Index"),                
            };
        }

    }

}
