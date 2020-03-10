using System;
using Microsoft.Extensions.Localization;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Attachments.Navigation
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
                .Add(T["Attachments"], int.MaxValue - 15, configuration => configuration
                    .IconCss("fal fa-paperclip")
                    .Add(T["Manage"], int.MinValue + 30, installed => installed
                        .Action("Index", "Admin", "Plato.Attachments")
                        .Permission(Permissions.ManageAttachments)
                        .LocalNav()
                    ).Add(T["Settings"], int.MinValue + 30, installed => installed
                       .Action("Settings", "Admin", "Plato.Attachments")
                       .Permission(Permissions.ManageAttachmentSettings)
                       .LocalNav()
                    ));

        }

    }

}
