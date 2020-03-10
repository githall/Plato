﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlatoCore.Net.Abstractions;

namespace PlatoCore.Net.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoNet(
            this IServiceCollection services)
        {

            services.TryAddScoped<IHttpMultiPartRequestHandler, HttpMultiPartRequestHandler>();
            services.TryAddScoped<IClientIpAddress, ClientIpAddress>();
            services.TryAddScoped<IHttpClient, HttpClient>();
            return services;

        }

    }

}
