using System.Collections.Generic;
using PlatoCore.Abstractions.Routing;

namespace Plato.Docs
{

    public class HomeRoutes : IHomeRouteProvider
    {
        public IEnumerable<HomeRoute> GetRoutes()
        {
            return new[]
            {
                new HomeRoute("Plato.Docs", "Home", "Index")
            };
        }

    }

}
