using System.Collections.Generic;
using PlatoCore.Abstractions.Routing;

namespace Plato.Questions
{

    public class HomeRoutes : IHomeRouteProvider
    {
        public IEnumerable<HomeRoute> GetRoutes()
        {
            return new[]
            {
                new HomeRoute("Plato.Questions", "Home", "Index")
            };
        }

    }

}
