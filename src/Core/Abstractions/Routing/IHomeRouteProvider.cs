using System.Collections.Generic;

namespace PlatoCore.Abstractions.Routing
{

    public interface IHomeRouteProvider
    {
        IEnumerable<HomeRoute> GetRoutes();
    }

}
