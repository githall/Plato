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
                .Add(T["Settings"], int.MaxValue, configuration => configuration
                    .IconCss("fal fa-paper-clip")
                    .Add(T["Attachments"], int.MinValue + 30, installed => installed
                        .Action("Index", "Admin", "Plato.Attachments")
                        .Permission(Permissions.ManageAttachmentSettings)
                        .LocalNav()
                    ));

        }

    }

}
