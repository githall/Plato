using System;
using Microsoft.Extensions.Localization;
using Plato.Entities.Ratings.ViewModels;
using PlatoCore.Navigation.Abstractions;
using Plato.Issues.Models;

namespace Plato.Issues.Votes.Navigation
{

    public class IssueCommentDetailsMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }

        public IssueCommentDetailsMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "issue-comment-details", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get entity from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Issue)] as Issue;
            if (entity == null)
            {
                return;
            }

            // Get reply from navigation builder
            var reply = builder.ActionContext.HttpContext.Items[typeof(Comment)] as Comment;
            if (reply == null)
            {
                return;
            }

            // Add vote toggle view to navigation
            builder
            .Add(T["Vote"], react => react
                    .View("VoteToggle", new
                    {
                        model = new VoteToggleViewModel()
                        {
                            Entity = entity,
                            Reply = reply,
                            Permission = Permissions.VoteReplies,
                            ApiUrl = builder.ActionContext.HttpContext.Request.PathBase + "/api/issues/vote/post"
                        }
                    })
            );

        }

    }

}
