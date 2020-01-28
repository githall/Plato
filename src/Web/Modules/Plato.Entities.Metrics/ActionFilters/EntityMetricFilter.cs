using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PlatoCore.Layout;
using PlatoCore.Layout.ActionFilters;
using PlatoCore.Models.Users;
using PlatoCore.Net.Abstractions;
using Plato.Entities.Metrics.Models;
using Plato.Entities.Metrics.Services;

namespace Plato.Entities.Metrics.ActionFilters
{

    public class EntityMetricFilter : IModularActionFilter
    {

        public const string UserAgentHeader = "User-Agent";

        private readonly IEntityMetricsManager<EntityMetric> _entityMetricManager;
        private readonly IClientIpAddress _clientIpAddress;

        public EntityMetricFilter(
            IEntityMetricsManager<EntityMetric> entityMetricManager,
            IClientIpAddress clientIpAddress)
        {
            _entityMetricManager = entityMetricManager;
            _clientIpAddress = clientIpAddress;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            return;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }
        
        public Task OnActionExecutingAsync(ResultExecutingContext context)
        {
            return Task.CompletedTask;
        }

        public async Task OnActionExecutedAsync(ResultExecutingContext context)
        {

            // The controller action didn't return a view result so no need to continue execution
            var result = context.Result as ViewResult;

            // Check early to ensure we are working with a LayoutViewModel
            var model = result?.Model as LayoutViewModel;
            if (model == null)
            {
                return;
            }

            // Check for the id route value 
            var id = context.RouteData.Values["opts.id"];
            if (id == null)
            {
                return;
            }

            // To int
            var ok = int.TryParse(id.ToString(), out var entityId);

            // Id route value is not a valid int
            if (!ok)
            {
                return;
            }

            // Get authenticated user from context
            var user = context.HttpContext.Features[typeof(User)] as User;

            // Add metric
            await _entityMetricManager.CreateAsync(new EntityMetric()
            {
                EntityId = entityId,
                IpV4Address = _clientIpAddress.GetIpV4Address(),
                IpV6Address = _clientIpAddress.GetIpV6Address(),
                UserAgent = context.HttpContext.Request.Headers.ContainsKey(UserAgentHeader)
                    ? context.HttpContext.Request.Headers[UserAgentHeader].ToString()
                    : string.Empty,
                CreatedUserId = user?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            });

        }
    }

}
