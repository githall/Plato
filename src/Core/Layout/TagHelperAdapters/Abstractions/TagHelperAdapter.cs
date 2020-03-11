using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PlatoCore.Layout.TagHelperAdapters.Abstractions
{

    public interface ITagHelperAdapter
    {

        string ViewName { get; }

        string TagId { get; }

        Task ProcessAsync(TagHelperContext context, TagHelperOutput output);

    }

    public class TagHelperAdapter : ITagHelperAdapter
    {

        public string ViewName { get; private set; }

        public string TagId { get; private set; }

        readonly Func<TagHelperContext, TagHelperOutput, Task> _adapter;

        public TagHelperAdapter(string viewName, string tagId, Func<TagHelperContext, TagHelperOutput, Task> adapter)
        {
            ViewName = viewName;
            TagId = tagId;
            _adapter = adapter;
        }

        public TagHelperAdapter(string viewName, string tagId, Action<TagHelperContext, TagHelperOutput> adapter)
        {
            ViewName = viewName;
            TagId = tagId;
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
