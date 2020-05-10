using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PlatoCore.Layout.Views.Abstractions;
using PlatoCore.Theming.Abstractions;

namespace PlatoCore.Layout.TagHelpers
{

    [HtmlTargetElement("layout")]
    public class LayoutTagHelper : TagHelper
    {

        [HtmlAttributeName("name")]
        public string Name { get; set; }

        [HtmlAttributeName("model")]
        public LayoutViewModel Model { get; set; }

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }
        
        private IViewDisplayHelper _viewDisplayHelper;
        
        private readonly IViewHelperFactory _viewHelperFactory;
        private readonly IThemeSelector _themeSelector;

        public LayoutTagHelper(
            IViewHelperFactory viewHelperFactory,
            IThemeSelector themeSelector)
        {
            _viewHelperFactory = viewHelperFactory;
            _themeSelector = themeSelector;
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

        // --------------

        private void EnsureViewHelper()
        {
            if (_viewDisplayHelper == null)
            {
                _viewDisplayHelper = _viewHelperFactory.CreateHelper(ViewContext);
            }
        }

        private async Task<IHtmlContent> Build()
        {

            EnsureViewHelper();

            var model = new LayoutOptions()
            {
                Zones = Model
            };

            var builder = new HtmlContentBuilder();
            try
            {
                builder.AppendHtml(await _viewDisplayHelper.DisplayAsync(new View(GetZoneLayoutPath(), model)));
            }
            catch
            {
                throw;
            }          

            return builder;

        }

        private string GetZoneLayoutPath()
        {       

            // If we have an explicit layout use it
            if (!string.IsNullOrEmpty(Name))
            {
                return $"~/{_themeSelector.GetThemePath()}/Shared/{Name}.cshtml";
            }

            // Else fall back to our default layouts
            switch (ViewContext.RouteData.Values["controller"].ToString())
            {
                case "Admin":
                    return $"~/{_themeSelector.GetThemePath()}/Shared/_AdminZoneLayout.cshtml";                  
                default:
                    return $"~/{_themeSelector.GetThemePath()}/Shared/_ZoneLayout.cshtml";               
            }

        }

    }

}
