using System;
using System.Threading.Tasks;
using Plato.Questions.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using Microsoft.AspNetCore.Mvc.Localization;
using PlatoCore.Layout.TagHelperAdapters.Abstractions;

namespace Plato.Questions.New.ViewAdapters
{

    public class QuestionAnswerListItemViewAdapter : ViewAdapterProviderBase
    {   

        public IHtmlLocalizer T { get; }

        public QuestionAnswerListItemViewAdapter(IHtmlLocalizer<QuestionListItemViewAdapter> localizer)
        {

            T = localizer;
            ViewName = "QuestionAnswerListItem";

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
                v.AdaptModel<EntityReplyListItemViewModel<Question, Answer>>(model =>
                {

                    // Build tag adapters
                    var adapters = new[]
                    {
                        new TagHelperAdapter("title", (context, output) =>
                        {
                            output.PostElement.SetHtmlContent(
                                        $"<span class=\"badge badge-primary ml-2\">{T["Question Comment List Item View Adapter"].Value}</span>");
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
