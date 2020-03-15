using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Docs.Models;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Docs.Flipper.Navigation
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
            var reply = builder.ActionContext.HttpContext.Items[typeof(DocComment)] as DocComment;

            // Add reaction list to navigation
            builder
                .Add(T["DocFlipper"], int.MaxValue - 10, flipper => flipper
                    .View("DocFlipper", new
                    {
                      entity,
                      reply
                    })
                    //.Permission(Permissions.ViewDocReactions)
                );


        }

    }

}
