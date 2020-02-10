using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PlatoCore.Layout.TagHelperAdapters.Abstractions{

    public interface ITagHelperAdapter
    {

        string Id { get; set; }

        Task ProcessAsync(TagHelperContext context, TagHelperOutput output);

    }

    public class TagHelperAdapter : ITagHelperAdapter
    {

        public string Id { get; set; }

        readonly Func<TagHelperContext, TagHelperOutput, Task> _adapter;

        public TagHelperAdapter(string id, Func<TagHelperContext, TagHelperOutput, Task> adapter)
        {
            Id = id;
            _adapter = adapter;
        }

        public TagHelperAdapter(string id, Action<TagHelperContext, TagHelperOutput> adapter)
        {
            Id = id;
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
