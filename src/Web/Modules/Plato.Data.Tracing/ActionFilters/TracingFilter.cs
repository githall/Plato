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

            // Add our trace view to the layout
            // It's important to add this after the layout has rendered
            // This allows all queries to execute before rendering the trace list
            if (context.Controller is Controller controller)
            {
                var updater = await _layoutUpdater.GetLayoutAsync(controller);
                await updater.UpdateLayoutAsync(async layout =>
                {
                    layout.LayoutAfter = await updater.UpdateZoneAsync(layout.LayoutAfter, zone =>
                    {
                        zone.Add(new LayoutZoneView("DbTraceList", new {}).Order(int.MaxValue));
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
