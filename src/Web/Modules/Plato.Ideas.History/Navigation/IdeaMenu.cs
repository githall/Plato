﻿using System;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Ideas.Models;
using Plato.Entities.History.ViewModels;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;

namespace Plato.Ideas.History.Navigation
{

    public class IdeaMenu : INavigationProvider
    {

        private readonly IContextFacade _contextFacade;

        public IStringLocalizer T { get; set; }

        public IdeaMenu(
            IStringLocalizer localizer,
            IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "idea", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Idea)] as Idea;
            if (entity == null)
            {
                return;
            }

            // No edited information
            if (entity.EditedDate == null)
            {
                return;
            }

            // Add HistoryMenu view to entity
            builder
                .Add(T["History"], int.MinValue, history => history
                    .View("HistoryMenu", new
                    {
                        model = new HistoryMenuViewModel()
                        {
                            Entity = entity,
                            DialogUrl = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                            {
                                ["area"] = "Plato.Ideas.History",
                                ["controller"] = "Home",
                                ["action"] = "Index",
                                ["id"] = 0
                            })
                        }
                    })
                    .Permission(Permissions.ViewEntityHistory)
                );

        }

    }

}
