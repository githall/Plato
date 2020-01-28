using System;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Discuss.Tags.Navigation
{

    public class TopicReplyFooterMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public TopicReplyFooterMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "topic-reply-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Topic)] as Topic;

            // We need an entity
            if (entity == null)
            {
                return;
            }

            // Replies are optional
            var reply = builder.ActionContext.HttpContext.Items[typeof(Reply)] as Reply;

            // Add reaction list to topic reply footer navigation
            builder
                .Add(T["Tags"], react => react
                    .View("TopicTags", new
                    {
                        entity,
                        reply
                    })
                    .Order(1)
                );

        }

    }

}
