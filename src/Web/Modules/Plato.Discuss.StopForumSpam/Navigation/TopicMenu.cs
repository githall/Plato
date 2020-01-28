using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Discuss.StopForumSpam.Navigation
{
    public class TopicMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public TopicMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "topic", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            
            // Get entity from context
            var entity = builder.ActionContext.HttpContext.Items[typeof(Topic)] as Topic;
            if (entity == null)
            {
                return;
            }
            
            // If the entity if flagged as spam display additional options
            if (entity.IsSpam)
            {

                builder
                    .Add(T["StopForumSpam"], int.MinValue, options => options
                            .IconCss("fal fa-hand-paper")
                            .Attributes(new Dictionary<string, object>()
                            {
                                {"data-toggle", "tooltip"},
                                {"title", T["Spam Options"]},
                                {"data-provide", "dialog"},
                                {"data-dialog-modal-css", "modal fade"},
                                {"data-dialog-css", "modal-dialog modal-lg"}
                            })
                            .Action("Index", "Home", "Plato.Discuss.StopForumSpam",
                                new RouteValueDictionary()
                                {
                                    ["opts.id"] = entity.Id.ToString(),
                                    ["opts.alias"] = entity.Alias,
                                    ["opts.replyId"] = "0"
                                })
                            .Permission(Permissions.ViewStopForumSpam)
                            .LocalNav()
                        , new List<string>() {"topic-stop-forum-spam", "text-muted", "text-hidden"}
                    );
            }

        }

    }

}
