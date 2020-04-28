﻿using System;
using Microsoft.Extensions.Localization;
using Plato.Entities.Ratings.ViewModels;
using PlatoCore.Navigation.Abstractions;
using Plato.Discuss.Models;

namespace Plato.Discuss.Votes.Navigation
{

    public class TopicDetailsMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }

        public TopicDetailsMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "topic-details", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Topic)] as Topic;
            if (entity == null)
            {
                return;
            }

            // Add reaction menu view to navigation
            builder
                .Add(T["Vote"], react => react
                    .View("VoteToggle", new
                    {
                        model = new VoteToggleViewModel()
                        {
                            Entity = entity,
                            Permission = Permissions.VoteTopics,
                            ApiUrl = builder.ActionContext.HttpContext.Request.PathBase + "/api/discuss/vote/post"
                        }
                    })
                );

        }

    }

}
