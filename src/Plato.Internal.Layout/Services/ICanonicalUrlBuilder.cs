using Microsoft.AspNetCore.Routing;

namespace Plato.Internal.Layout.Services
{
    public interface ICanonicalUrlBuilder
    {

        string Url { get; }

        RouteValueDictionary RouteValues { get; }

        ICanonicalUrlBuilder AddUrl(string url);

        ICanonicalUrlBuilder AddRoute(RouteValueDictionary routeValues);

    }

}
