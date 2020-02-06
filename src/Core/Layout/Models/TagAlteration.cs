using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PlatoCore.Layout.Models
{

    public class TagAlteration
    {

        public string Id { get; set; }

        readonly Func<TagHelperContext, TagHelperOutput, Task> _alteration;

        public TagAlteration(string id, Func<TagHelperContext, TagHelperOutput, Task> alteration)
        {
            Id = id;
            _alteration = alteration;
        }

        public TagAlteration(string id, Action<TagHelperContext, TagHelperOutput> alteration)
        {
            Id = id;
            // Wrap action wtihin func
            _alteration = (context, output) =>
            {
                alteration.Invoke(context, output);
                return Task.CompletedTask;
            };
        }

        public async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await _alteration.Invoke(context, output);
        }

    }

}
