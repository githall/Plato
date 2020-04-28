using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Cache.Abstractions;

namespace PlatoCore.Cache.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoCaching(
            this IServiceCollection services)
        {

            // Same as services.AddMemoryCache();
            services.Add(ServiceDescriptor.Singleton<IMemoryCache, MemoryCache>());

            // Not implemented - Can be swapped out with real RedisCache
            // I.e. services.AddDistributedRedisCache(options => {});
            services.Add(ServiceDescriptor.Transient<IDistributedCache, MemoryDistributedCache>());

            // Cache dependency
            services.AddSingleton<ICacheDependency, CacheDependency>();

            // A host level cache
            services.AddSingleton<ICacheManager, SimpleCacheManager>();                 

            return services;

        }

        public static IServiceCollection AddShellCaching(
            this IServiceCollection services)
        {

            // This replaces the host instance of IMemoryCache at the tenant level
            // This is critical to ensure each tenant has it's own isolated singleton instance of IMemoryCache
            services.Add(ServiceDescriptor.Singleton<IMemoryCache, MemoryCache>());

            // Add cache manager
            services.AddSingleton<ICacheManager, SimpleCacheManager>();

            return services;
        }

    }

}
