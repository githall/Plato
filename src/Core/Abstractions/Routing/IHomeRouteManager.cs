using System.Collections.Generic;

namespace PlatoCore.Abstractions.Routing
{

    public interface IHomeRouteManager
    {

        IEnumerable<HomeRoute> GetDefaultRoutes();
    }

}
