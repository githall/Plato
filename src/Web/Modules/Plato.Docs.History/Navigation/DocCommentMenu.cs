﻿using System;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Docs.Models;
using Plato.Entities.History.ViewModels;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Docs.History.Navigation
{
    public class DocCommentMenu : INavigationProvider
    {

        private readonly IContextFacade _contextFacade;

        public IStringLocalizer T { get; set; }

        public DocCommentMenu(
            IStringLocalizer localizer,
            IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "doc-comment", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Doc)] as Doc;
            var reply = builder.ActionContext.HttpContext.Items[typeof(DocComment)] as DocComment;

            if (reply == null)
            {
                return;
            }

            // No edited information
            if (reply.EditedDate == null)
            {
                return;
            }

            // Add HistoryMenu view to reply
            builder
                .Add(T["History"], int.MinValue, history => history
                    .View("HistoryMenu", new
                    {
                        model = new HistoryMenuViewModel()
                        {
                            Entity = entity,
                            Reply = reply,
                            DialogUrl = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                            {
                                ["area"] = "Plato.Docs.History",
                                ["controller"] = "Home",
                                ["action"] = "Index",
                                ["id"] = 0
                            })
                        }
                    })
                    .Permission(Permissions.viewReplyHistory)
                );
            
        }

    }

}
