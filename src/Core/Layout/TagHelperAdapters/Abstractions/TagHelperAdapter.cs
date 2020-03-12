using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PlatoCore.Layout.TagHelperAdapters.Abstractions
{

    public interface ITagHelperAdapter
    {

        string ViewName { get; }

        string TagHelperId { get; }

        Task ProcessAsync(TagHelperContext context, TagHelperOutput output);

    }

    public class TagHelperAdapter : ITagHelperAdapter
    {

        public string ViewName { get; private set; }

        public string TagHelperId { get; private set; }

        readonly Func<TagHelperContext, TagHelperOutput, Task> _adapter;

        public TagHelperAdapter(string viewName, string tagHelperId, Func<TagHelperContext, TagHelperOutput, Task> adapter)
        {
            ViewName = viewName;
            TagHelperId = tagHelperId;
            _adapter = adapter;
        }

        public TagHelperAdapter(string viewName, string tagHelperId, Action<TagHelperContext, TagHelperOutput> adapter)
        {
            ViewName = viewName;
            TagHelperId = tagHelperId;
            // Wrap action wtihin func
            _adapter = (context, output) =>
            {
                adapter.Invoke(context, output);
                return Task.CompletedTask;
            };
        }

        public async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await _adapter(context, output);
        }

    }

}
