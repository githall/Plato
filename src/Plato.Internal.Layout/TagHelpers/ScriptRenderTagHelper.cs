using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Scripting.Abstractions;
using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.TagHelpers
{

    [HtmlTargetElement("scripts", Attributes = "section")]
    public class ScriptRenderTagHelper : TagHelper
    {

        const string ScriptTag = "script";

        [HtmlAttributeName("section")]
        public ScriptSection Section { get; set; }

        [HtmlAttributeName("auto-merge")]
        public bool AutoMerge { get; set; }

        private readonly IScriptManager _scriptManager;

        public ScriptRenderTagHelper(IScriptManager scriptManager)
        {
            _scriptManager = scriptManager;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            output.TagName = null;

            var capture = _scriptManager.GetScriptBlocks(Section);
            if (capture == null)
            {
                output.SuppressOutput();
                return;
            }

            var result = new HelperResult(async textWriter => await RenderBlocks(textWriter, capture));
            output.Content.SetHtmlContent(result);

        }

        async Task RenderBlocks(TextWriter textWriter, ScriptBlocks blocks)
        {

            var orderedBlocks = blocks.Blocks.OrderBy(b => b.Order);
            var orderedBlocksList = orderedBlocks.ToList();

            // Get blocks we can merge
            var mergableBlocks = orderedBlocksList.Where(b => (
                (AutoMerge && b.CanMerge) ||
                (!AutoMerge && b.CanMerge))
            );

            // Convert mergables to list
            var mergableBlocksList = mergableBlocks.ToList();

            // Create a final list of blocks we need to merge
            var finalList = mergableBlocksList.ToList();

            // Render merged blocks
            await RenderMergedBlocks(textWriter, finalList);

            // Get all other blocks we can't merge
            var otherBlocks = orderedBlocksList.Except(mergableBlocksList);

            // Render blocks we can't merge
            await RenderSeparateBlocks(textWriter, otherBlocks);

        }

        async Task RenderSeparateBlocks(TextWriter textWriter, IEnumerable<ScriptBlock> blocks)
        {

            foreach (var block in blocks)
            {

                if (block.DisableScriptTag)
                {
                    var builder = new HtmlContentBuilder();
                    builder.AppendHtml(block.Content);
                    builder.WriteTo(textWriter, NullHtmlEncoder.Default);
                    await textWriter.WriteLineAsync();
                }
                else
                {

                    var tagBuilder = new TagBuilder(ScriptTag)
                    {
                        TagRenderMode = TagRenderMode.Normal
                    };

                    tagBuilder.InnerHtml.AppendHtml(block.Content);
                    tagBuilder.MergeAttributes(block.Attributes, replaceExisting: true);
                    tagBuilder.WriteTo(textWriter, NullHtmlEncoder.Default);
                    await textWriter.WriteLineAsync();

                }

            }

        }

        async Task RenderMergedBlocks(TextWriter textWriter, IEnumerable<ScriptBlock> blocks)
        {

            var blockList = blocks.ToList();

            if (!blockList.Any())
            {
                return;
            }

            var tagBuilder = new TagBuilder(ScriptTag)
            {
                TagRenderMode = TagRenderMode.Normal
            };

            foreach (var block in blockList)
            {
                tagBuilder.InnerHtml.AppendHtml(block.Content);                    
                tagBuilder.MergeAttributes(block.Attributes, replaceExisting: true);
            }

            tagBuilder.WriteTo(textWriter, NullHtmlEncoder.Default);
            await textWriter.WriteLineAsync();

        }

    }

}
