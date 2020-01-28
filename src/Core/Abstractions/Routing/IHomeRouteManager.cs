using System;
using System.Collections.Generic;
using System.Text;

namespace PlatoCore.Abstractions.Routing
{
    public interface IHomeRouteManager
    {

        IEnumerable<HomeRoute> GetDefaultRoutes();
    }

}
