using System;
using Microsoft.Extensions.Localization;
using PlatoCore.Models.Extensions;
using PlatoCore.Models.Shell;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Shell.Abstractions;

namespace Plato.Tenants.Navigation
{
    public class AdminMenu : INavigationProvider
    {

        private readonly IShellSettings _shellSettings;

        public AdminMenu
            (IStringLocalizer<AdminMenu> localizer,
            IShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Ensure host
            if (!_shellSettings.IsDefaultShell())
            {
                return;
            }

            builder
                .Add(T["Tenants"], int.MaxValue - 8, tenants => tenants
                    .IconCss("fal fa-browser")
                    .Add(T["Manage"], 3, manage => manage
                        .Action("Index", "Admin", "Plato.Tenants")
                        .Permission(Permissions.ManageTenants)
                        .LocalNav())
                    .Add(T["Add"], 4, create => create
                        .Action("Create", "Admin", "Plato.Tenants")
                        .Permission(Permissions.AddTenants)
                        .LocalNav())
                    .Add(T["Settings"], 5, settings => settings
                        .Action("Settings", "Admin", "Plato.Tenants")
                        .Permission(Permissions.EditSettings)
                        .LocalNav())
                );

        }

    }

}
