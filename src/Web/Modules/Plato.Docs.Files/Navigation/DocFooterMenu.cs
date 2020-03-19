﻿using System;
using Microsoft.Extensions.Localization;
using Plato.Docs.Models;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Docs.Files.Navigation
{

    public class DocFooterMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public DocFooterMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "doc-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Doc)] as Doc;

            // We always need a topic
            if (entity == null)
            {
                return;
            }

            // Replies are options
            var reply = builder.ActionContext.HttpContext.Items[typeof(DocComment)] as DocComment;

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
