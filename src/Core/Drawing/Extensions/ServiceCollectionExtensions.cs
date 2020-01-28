using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Drawing.Abstractions;
using PlatoCore.Drawing.Abstractions.Letters;
using PlatoCore.Drawing.Letters;

namespace PlatoCore.Drawing.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoDrawing(
            this IServiceCollection services)
        {

            services.AddScoped<IDisposableBitmap, DisposableBitmap>();
            services.AddScoped<IInMemoryLetterRenderer, InMemoryLetterRenderer>();

            return services;

        }


    }
}
