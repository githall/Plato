using System.Collections.Generic;
using PlatoCore.Abstractions.Routing;

namespace Plato.Articles
{

    public class HomeRoutes : IHomeRouteProvider
    {
        public IEnumerable<HomeRoute> GetRoutes()
        {
            return new[]
            {
                new HomeRoute("Plato.Articles", "Home", "Index")
            };
        }

    }

}
