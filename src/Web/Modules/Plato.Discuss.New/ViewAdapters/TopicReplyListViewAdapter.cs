﻿using System;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using Microsoft.AspNetCore.Mvc.Localization;
using PlatoCore.Layout.TagHelperAdapters.Abstractions;

namespace Plato.Discuss.New.ViewAdapters
{

    public class TopicReplyListViewAdapter : ViewAdapterProviderBase
    {   

        public IHtmlLocalizer T { get; }

        public TopicReplyListViewAdapter(IHtmlLocalizer<TopicListItemViewAdapter> localizer)
        {

            T = localizer;
            ViewName = "TopicReplyList";

        }

        public override async Task<IViewAdapterResult> ConfigureAsync(string viewName)
        {

            // Ensure adapter is for current view
            if (!viewName.Equals(ViewName, StringComparison.OrdinalIgnoreCase))
            {
                return default(IViewAdapterResult);
            }

            // Adapt the view
            return await AdaptAsync(ViewName, v =>
            {
                v.AdaptModel<EntityViewModel<Topic, Reply>>(model =>
                {

                    // Build tag adapters
                    var adapters = new[]
                    {
                        new TagHelperAdapter(ViewName, "title", (context, output) =>
                        {
                            output.Attributes.SetAttribute("class", "d-inline-block");
                            output.PostElement.SetHtmlContent(
                                        $"<span class=\"badge badge-primary ml-2\">{T["Topic Reply List View Adapter"].Value}</span>");
                        })
                    };

                    // Add tag adapters
                    model.TagHelperAdapters.Add(adapters);

                    // Return an anonymous type, we are adapting a view component
                    return Task.FromResult((object)new
                    {
                        model
                    });

                });
            });

        }

    }

}
