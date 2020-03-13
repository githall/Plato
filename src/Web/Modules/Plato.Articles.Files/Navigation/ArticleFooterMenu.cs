using System;
using Microsoft.Extensions.Localization;
using Plato.Articles.Models;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Articles.Files.Navigation
{

    public class ArticleFooterMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public ArticleFooterMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "article-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Article)] as Article;

            // We always need a topic
            if (entity == null)
            {
                return;
            }

            // Replies are options
            var reply = builder.ActionContext.HttpContext.Items[typeof(Comment)] as Comment;

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
