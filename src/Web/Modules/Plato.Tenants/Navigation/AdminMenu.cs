using System;
using Microsoft.Extensions.Localization;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Tenants.Navigation
{
    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Tenants"], int.MaxValue - 8, tenants => tenants
                    .IconCss("fal fa-browser")
                    .Add(T["Manage"], 3, manage => manage
                        .Action("Index", "Admin", "Plato.Tenants")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav())
                    .Add(T["Add"], 4, create => create
                        .Action("Create", "Admin", "Plato.Tenants")
                        //.Permission(Permissions.AddRoles)
                        .LocalNav())
                );

        }

    }

}
