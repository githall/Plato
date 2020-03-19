using System;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Discuss.Files.Navigation
{

    public class TopicFooterMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public TopicFooterMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "topic-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Topic)] as Topic;

            // We always need a topic
            if (entity == null)
            {
                return;
            }

            // Replies are options
            var reply = builder.ActionContext.HttpContext.Items[typeof(Reply)] as Reply;

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
