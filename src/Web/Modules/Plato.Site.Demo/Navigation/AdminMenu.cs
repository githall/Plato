using System;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using PlatoCore.Abstractions.Settings;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Site.Demo.Navigation
{

    public class AdminMenu : INavigationProvider
    {

        private readonly PlatoOptions _platoOptions;

        public AdminMenu(
            IStringLocalizer<AdminMenu> localizer,
            IOptions<PlatoOptions> platoOptions)
        {
            T = localizer;
            _platoOptions = platoOptions.Value;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Hidden whilst in demo mode, still accessible via direct Url
            if (_platoOptions.DemoMode)
            {
                return;
            }

            builder
                .Add(T["Settings"], int.MaxValue, settings => settings
                    .IconCss("fal fa-cog")
                    .Add(T["Demo"], int.MaxValue - 450, demo => demo
                        .Action("Index", "Admin", "Plato.Site.Demo")                        
                        .LocalNav())
                );

        }

    }

}
