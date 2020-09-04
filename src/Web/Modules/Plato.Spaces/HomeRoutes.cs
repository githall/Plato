using System.Collections.Generic;
using PlatoCore.Abstractions.Routing;

namespace Plato.Spaces
{

    public class HomeRoutes : IHomeRouteProvider
    {
        public IEnumerable<HomeRoute> GetRoutes()
        {
            return new[]
            {
                new HomeRoute("Plato.Spaces", "Home", "Index")
            };
        }

    }

}
