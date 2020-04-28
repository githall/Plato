using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlatoCore.Http.Abstractions;

namespace PlatoCore.Http.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddShellHttp(
            this IServiceCollection services)
        {

            services.TryAddScoped<IHttpMultiPartRequestHandler, HttpMultiPartRequestHandler>();
            services.TryAddScoped<IClientIpAddress, ClientIpAddress>();
            services.TryAddScoped<IHttpClient, HttpClient>();
            services.TryAddScoped<ICookieBuilder, CookieBuilder>();

            return services;

        }

    }

}
