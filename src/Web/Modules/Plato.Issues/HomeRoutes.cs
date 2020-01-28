using System.Collections.Generic;
using PlatoCore.Abstractions.Routing;

namespace Plato.Issues
{

    public class HomeRoutes : IHomeRouteProvider
    {
        public IEnumerable<HomeRoute> GetRoutes()
        {
            return new[]
            {
                new HomeRoute("Plato.Issues", "Home", "Index")
            };
        }

    }

}
