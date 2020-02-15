using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PlatoCore.Data.Tracing.Abstractions;
using PlatoCore.Layout;
using PlatoCore.Layout.ActionFilters;
using PlatoCore.Layout.Views.Abstractions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Plato.Data.Tracing.ActionFilters
{
    public class TracingFilter : IModularActionFilter
    {

        private readonly ILayoutUpdater _layoutUpdater;

        public TracingFilter(ILayoutUpdater layoutUpdater)
        {
            _layoutUpdater = layoutUpdater;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {   
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {          
        }


        public async Task OnActionExecutingAsync(ResultExecutingContext context)
        {

            var state = context.HttpContext.RequestServices.GetService<IDbTraceState>();

            // Add trace information to the top of our content zone
            if (context.Controller is Controller controller)
            {
                var updater = await _layoutUpdater.GetLayoutAsync(controller);
                await updater.UpdateLayoutAsync(async layout =>
                {
                    layout.Content = await updater.UpdateZoneAsync(layout.Content, zone =>
                    {
                        zone.Add(new PositionedView("DbTrace", new { state.Traces }).Order(int.MinValue));                
                    });
                    return layout;
                });
            }

        }

        public Task OnActionExecutedAsync(ResultExecutingContext context)
        {
            return Task.CompletedTask;
        }

    }
}
