using System;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using PlatoCore.Models.Tour;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Tour.Navigation
{

    public class AdminMenu : INavigationProvider
    {

        private readonly TourOptions _tourOptions;

        public AdminMenu(            
            IStringLocalizer<AdminMenu> localizer,
            IOptions<TourOptions> tourOptions)
        {
            _tourOptions = tourOptions.Value;
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            // Tour is already complete, no need to add progress
            if (_tourOptions.Completed)
            {
                return;
            }

            // Ensure admin menu
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Add tour progress to administrator menu
            builder
                .Add(T["TourProgress"], int.MinValue + 30, progress => progress
                    .View("TourProgress", new {})                    
                );

        }

    }

}
