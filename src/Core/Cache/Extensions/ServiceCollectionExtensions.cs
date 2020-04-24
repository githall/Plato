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

            services.AddSingleton<ICacheDependency, CacheDependency>();
            services.AddSingleton<ISingletonCacheManager, SingletonCacheManager>();                 

            return services;

        }

        public static IServiceCollection AddShellCaching(
       this IServiceCollection services)
        {
            services.AddScoped<ICacheManager, ShellCacheManager>();
            return services;
        }

    }

}
