using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlatoCore.Models.Tour;
using PlatoCore.Stores.Abstractions.Tour;

namespace Plato.Tour.Configuration
{

    //public class TourDescriptorConfiguration : IConfigureOptions<TourDescriptor>
    //{

    //    private readonly ITourDescriptorStore _tourDescriptorStore;
    //    private readonly ILogger<TourDescriptorConfiguration> _logger;

    //    public TourDescriptorConfiguration(
    //        ITourDescriptorStore tourDescriptorStore,            
    //        ILogger<TourDescriptorConfiguration> logger)
    //    {
    //        _tourDescriptorStore = tourDescriptorStore;      
    //        _logger = logger;
    //    }

    //    public void Configure(TourDescriptor options)
    //    {

    //        var settings = _tourDescriptorStore
    //            .GetAsync()
    //            .GetAwaiter()
    //            .GetResult();

    //        // We have no settings to configure
    //        options = settings;

    //    }

    //}

}
