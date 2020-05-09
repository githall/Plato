using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout.TagHelpers
{

    [HtmlTargetElement("layout")]
    public class LayoutTagHelper : TagHelper
    {

        [HtmlAttributeName("name")]
        public string Name { get; set; } = "_PlatoLayout";


        [HtmlAttributeName("content-left-css")]
        public string ContentLeftCss { get; set; }

        [HtmlAttributeName("content-right-css")]
        public string ContentRightCss { get; set; }

        [HtmlAttributeName("model")]
        public LayoutViewModel Model { get; set; }

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }
        
        private IViewDisplayHelper _viewDisplayHelper;

   
        private readonly IViewHelperFactory _viewHelperFactory;
      
        public LayoutTagHelper(IViewHelperFactory viewHelperFactory)
        {
            _viewHelperFactory = viewHelperFactory;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            if (Model == null)
            {
                throw new ArgumentNullException(nameof(Model));
            }

            IHtmlContent builder = null;
            try
            {
                builder = await Build();
                if (builder == null)
                {
                    throw new Exception(
                        $"An error occurred whilst attempting to activate the layout \"{Name}\".");
                }
            }
            catch (Exception e)
            {
                builder = new HtmlContentBuilder()
                    .AppendHtml("An error occurred whilst attempting to invoke the layout \"")
                    .Append(Name)
                    .Append("\". ")
                    .AppendHtml("Details follow...<hr/><strong>Exception message:</strong> ")
                    .Append(e.Message)
                    .AppendHtml("<br/><strong>Stack trace:</strong> ")
                    .Append(Environment.NewLine)
                    .Append(Environment.NewLine)
                    .Append(e.StackTrace);
            }

            output.TagName = "";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(builder);

        }

        void EnsureViewHelper()
        {
            if (_viewDisplayHelper == null)
            {
                _viewDisplayHelper = _viewHelperFactory.CreateHelper(ViewContext);
            }
        }

        async Task<IHtmlContent> Build()
        {

            EnsureViewHelper();

            var model = new LayoutOptions()
            {
                Zones = Model
            };

            if (!string.IsNullOrEmpty(ContentLeftCss))
            {
                model.ContentLeftCss = ContentLeftCss;
            }

            if (!string.IsNullOrEmpty(ContentRightCss))
            {
                model.ContentRightCss = ContentRightCss;
            }
            
            var builder = new HtmlContentBuilder();
            try
            {
                var result = await _viewDisplayHelper.DisplayAsync(new View(Name, model));
                builder.AppendHtml(result);
            }
            catch
            {
                throw;
            }          

            return builder;

        }

    }

}
