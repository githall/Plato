using System.Collections.Generic;
using PlatoCore.Abstractions.Routing;

namespace Plato.Ideas
{

    public class HomeRoutes : IHomeRouteProvider
    {
        public IEnumerable<HomeRoute> GetRoutes()
        {
            return new[]
            {
                new HomeRoute("Plato.Ideas", "Home", "Index")
            };
        }

    }

}
