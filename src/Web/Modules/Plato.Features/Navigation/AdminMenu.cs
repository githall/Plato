﻿using System;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Features.Navigation
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
                .Add(T["Features"], int.MaxValue - 10, features => features
                    .IconCss("fal fa-cube")
                    .Add(T["Manage"], 1, manage => manage
                        .Action("Index", "Admin", "Plato.Features", new RouteValueDictionary()
                        {
                            ["opts.category"] = "all"
                        })
                        .Permission(Permissions.ManageFeatures)
                        .LocalNav())
                );

        }

    }

}
