using System;
using System.Threading.Tasks;
using Plato.Issues.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.ViewAdapters.Abstractions;
using Microsoft.AspNetCore.Mvc.Localization;
using PlatoCore.Layout.TagHelperAdapters.Abstractions;

namespace Plato.Issues.New.ViewAdapters
{

    public class IssueListViewAdapter : ViewAdapterProviderBase
    {   

        public IHtmlLocalizer T { get; }

        public IssueListViewAdapter(IHtmlLocalizer<IssueListItemViewAdapter> localizer)
        {

            T = localizer;
            ViewName = "IssueList";

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
                v.AdaptModel<EntityIndexViewModel<Issue>>(model =>
                {

                    // Build tag adapters
                    var adapters = new[]
                    {
                        new TagHelperAdapter("title", (context, output) =>
                        {
                               output.PostElement.SetHtmlContent(
                                            $"<span class=\"badge badge-primary ml-2\">{T["Entity List View Adapter"].Value}</span>");

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
