using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Entities.Reactions.ViewModels;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Discuss.Reactions.Navigation
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
            
            if (entity == null)
            {
                return;
            }
            
            builder
                .Add(T["Reactions"], int.MaxValue, react => react
                    .View("ReactionList", new
                    {
                        model = new ReactionListViewModel()
                        {
                            Entity = entity,
                            Permission = Permissions.ReactToTopics
                        }
                    })
                    .Permission(Permissions.ViewTopicReactions)
                );

        }

    }

}
