﻿using System;
using Microsoft.Extensions.Localization;
using Plato.Ideas.Models;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Ideas.Files.Navigation
{

    public class IdeaFooterMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public IdeaFooterMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "idea-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Idea)] as Idea;

            // We always need a topic
            if (entity == null)
            {
                return;
            }

            // Replies are options
            var reply = builder.ActionContext.HttpContext.Items[typeof(IdeaComment)] as IdeaComment;

            builder
                .Add(T["Attachments"], react => react
                    .View("EntityFiles", new
                    {
                        entity,
                        reply
                    })
                );

        }

    }

}
