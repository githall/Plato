﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout.TagHelpers
{

    [HtmlTargetElement("view")]
    public class ViewTagHelper : TagHelper
    {


        public dynamic Model { get; set; }

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }
        
        private IViewDisplayHelper _viewDisplayHelper;

        /// <summary>
        /// Override the default order property to ensure our view tag helpers are
        /// always processed first. This is necessary to ensure the view tag helpers
        /// populate the TagHelperExecutionContext used throughout the request
        /// early in the request pipeline.
        /// https://github.com/aspnet/Razor/blob/1e5ad1154dd272cdfa4bdebab26468af28ea68b4/src/Microsoft.AspNet.Razor.Runtime/TagHelpers/TagHelperRunner.cs
        /// </summary>
        public override int Order => int.MinValue;

   
        private readonly IViewHelperFactory _viewHelperFactory;
      
        public ViewTagHelper(IViewHelperFactory viewHelperFactory)
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
                        $"An error occurred whilst attempting to activate a view. The supplied model of type {Model} is not a valid type. The supplied model must implement the IView interface.");
                }
            }
            catch (Exception e)
            {
                builder = new HtmlContentBuilder()
                    .AppendHtml("An error occurred whilst attempting to invoke a view. ")
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

            HtmlContentBuilder builder = null;
            
            if (Model is IEnumerable<IView>)
            {
                builder = new HtmlContentBuilder();
                foreach (var view in Model)
                {
                    try
                    {
                        var result = await _viewDisplayHelper.DisplayAsync(view);
                        builder.AppendHtml(result);
                    }
                    catch 
                    {
                        throw;
                    }
                }
            }
            else
            {
                if (Model is IView)
                {
                    builder = new HtmlContentBuilder();
                    try
                    {
                        var result = await _viewDisplayHelper.DisplayAsync(Model);
                        builder.AppendHtml(result);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            return builder;

        }

    }

}
