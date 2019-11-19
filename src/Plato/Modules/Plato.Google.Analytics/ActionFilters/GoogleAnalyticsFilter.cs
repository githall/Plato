using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Plato.Google.Models;
using Plato.Internal.Layout.ActionFilters;
using System.Threading.Tasks;

namespace Plato.Google.Analytics.ActionFilters
{
    public class GoogleAnalyticsFilter : IModularActionFilter
    {

        private readonly PlatoGoogleOptions _googleOptions;

        public GoogleAnalyticsFilter(IOptions<PlatoGoogleOptions> _googleOptionsAccessor)
        {
            _googleOptions = _googleOptionsAccessor.Value;
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
            return Task.CompletedTask;
        }

    }

}
