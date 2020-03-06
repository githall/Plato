using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Plato.Attachments.Services;
using PlatoCore.Layout;
using PlatoCore.Layout.ActionFilters;
using PlatoCore.Scripting.Abstractions;

namespace Plato.Attachments.ActionFilters
{
    public class AttachmentClientOptionsFilter : IModularActionFilter
    {

        public void OnActionExecuting(ActionExecutingContext context)
        {
            return;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }

        public async Task OnActionExecutingAsync(ResultExecutingContext context)
        {

            // The controller action didn't return a view result so no need to continue execution
            if (!(context.Result is ViewResult result))
            {
                return;
            }

            // Check early to ensure we are working with a LayoutViewModel
            if (!(result?.Model is LayoutViewModel model))
            {
                return;
            }

            // Register client options for web api with our script manager
            var scriptBlock = await BuildScriptBlockAsync(context.HttpContext);
            if (scriptBlock != null)
            {
                var scriptManager = context.HttpContext.RequestServices.GetRequiredService<IScriptManager>();
                scriptManager?.RegisterScriptBlock(scriptBlock, ScriptSection.Footer);
            }

        }

        public Task OnActionExecutedAsync(ResultExecutingContext context)
        {
            return Task.CompletedTask;
        }


        async Task<ScriptBlock> BuildScriptBlockAsync(HttpContext context)
        {

            var factory = context.RequestServices.GetRequiredService<IAttachmentOptionsFactory>();
            if (factory == null)
            {
                return null;
            }

            var settings = await factory.GetSettingsAsync();

            if (settings != null)
            {
                var script = "$(function (win) { $.extend(win.$.Plato.attachments, { allowedExtensions: {allowedExtensions}, maxFileSize: {maxFileSize}, availableSpace: {availableSpace} }); } (window));";
                script = script.Replace("{allowedExtensions}", BuildAllowedExtensions(settings.AllowedExtensions));
                script = script.Replace("{maxFileSize}", settings.MaxFileSize.ToString());
                script = script.Replace("{availableSpace}", settings.AvailableSpace.ToString());
                return new ScriptBlock(script);
            }

            return null;

        }

        string BuildAllowedExtensions(string[] extensions)
        {

            /* 
                [
                    "txt",
                    "html",
                    "zip",
                    "png",
                    "gif",
                    "bmp",
                    "jpg",
                    "jpeg",
                    "pdf"
                ],
            */

            var i = 0;
            var sb = new StringBuilder();
            sb.Append("[");
            foreach (var extension in extensions)
            {
                sb
                    .Append("\"")
                    .Append(extension)
                    .Append("\"");
                if (i < extensions.Length - 1)
                {
                    sb.Append(", ");
                }
                i++;
            }
            sb.Append("]");
            return sb.ToString();
        }

    }

}
