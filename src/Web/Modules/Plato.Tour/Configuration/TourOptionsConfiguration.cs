using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlatoCore.Models.Tour;
using PlatoCore.Stores.Abstractions.Tour;

namespace Plato.Tour.Configuration
{

    public class TourOptionsConfiguration : IConfigureOptions<TourOptions>
    {

        private readonly ITourDescriptorStore _tourDescriptorStore;
        private readonly ILogger<TourOptionsConfiguration> _logger;

        public TourOptionsConfiguration(
            ITourDescriptorStore tourDescriptorStore,
            ILogger<TourOptionsConfiguration> logger)
        {
            _tourDescriptorStore = tourDescriptorStore;
            _logger = logger;
        }

        public void Configure(TourOptions options)
        {

            var settings = _tourDescriptorStore
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            if (settings != null)
            {
                options.Completed = settings.Completed;
            }

        }

    }

}
