using System;
using Microsoft.Extensions.Localization;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Users.Navigation
{

    public class UserMenu : INavigationProvider
    {
        public UserMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "user", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["User"], user => user
                    .View("UserMenu", new {})
                );

        }

    }

}
