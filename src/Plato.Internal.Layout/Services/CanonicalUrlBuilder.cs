using Microsoft.AspNetCore.Routing;

namespace Plato.Internal.Layout.Services
{
    public class CanonicalUrlBuilder : ICanonicalUrlBuilder
    {

        private string _url;
        private RouteValueDictionary _routeValues;

        public string Url => _url;

        public RouteValueDictionary RouteValues => _routeValues;

        public ICanonicalUrlBuilder AddRoute(RouteValueDictionary routeValues)
        {
            _routeValues = routeValues;
            return this;
        }

        public ICanonicalUrlBuilder AddUrl(string url)
        {
            _url = url;
            return this;
        }

    }

}
