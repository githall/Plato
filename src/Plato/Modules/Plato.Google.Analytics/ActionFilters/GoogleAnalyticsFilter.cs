using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Plato.Google.Models;
using Plato.Internal.Layout.ActionFilters;
using Plato.Internal.Scripting.Abstractions;
using System.Threading.Tasks;

namespace Plato.Google.Analytics.ActionFilters
{
    public class GoogleAnalyticsFilter : IModularActionFilter
    {

        private readonly IScriptManager _scriptManager;        

        private readonly PlatoGoogleOptions _googleOptions;

        public GoogleAnalyticsFilter(
            IOptions<PlatoGoogleOptions> googleOptionsAccessor,
            IScriptManager scriptManager)
        {
            _googleOptions = googleOptionsAccessor.Value;
            _scriptManager = scriptManager;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }

        public Task OnActionExecutedAsync(ResultExecutingContext context)
        {
            return Task.CompletedTask;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            return;
        }

        public Task OnActionExecutingAsync(ResultExecutingContext context)
        {

            if (!ValidSetting())
            {
                return Task.CompletedTask;
            }

            // Should only run on the front-end for a full view
            if (context.Result is ViewResult || context.Result is PageResult)
            {
                _scriptManager?.RegisterScriptBlock(new ScriptBlock($"  <script src=\"https://www.googletagmanager.com/gtag/js?id={_googleOptions.TrackingId}\"></script>", true), ScriptSection.Footer);
                _scriptManager?.RegisterScriptBlock(new ScriptBlock($"  <script>window.dataLayer = window.dataLayer || [];function gtag() {{ dataLayer.push(arguments); }}gtag('js', new Date());gtag('config', '{_googleOptions.TrackingId}');</script>", int.MaxValue, true), ScriptSection.Footer);
            }

            return Task.CompletedTask;

        }

        bool ValidSetting()
        {
            return !string.IsNullOrEmpty(_googleOptions.TrackingId);
        }

    }

}
