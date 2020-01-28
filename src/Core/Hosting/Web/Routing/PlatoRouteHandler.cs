using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace PlatoCore.Hosting.Web.Routing
{

    public interface IPlatoRouteHandler : IRouter
    {

    }

    public class PlatoRouteHandler : IPlatoRouteHandler
    {
        private readonly IActionInvokerFactory _actionInvokerFactory;
        private readonly IActionSelector _actionSelector;
        private readonly ILogger _logger;
        private readonly DiagnosticListener _diagnosticListener;

        public PlatoRouteHandler(
            IActionInvokerFactory actionInvokerFactory,
            IActionSelector actionSelector,
            DiagnosticListener diagnosticListener,
            ILoggerFactory loggerFactory)
        {
            _actionInvokerFactory = actionInvokerFactory;
            _actionSelector = actionSelector;
            _diagnosticListener = diagnosticListener;
            _logger = loggerFactory.CreateLogger<PlatoRouteHandler>();
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // We return null here because we're not responsible for generating the url, the route is.
            return null;
        }

        public Task RouteAsync(RouteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var candidates = _actionSelector.SelectCandidates(context);
            if (candidates == null || candidates.Count == 0)
            {
                //_logger.NoActionsMatched(context.RouteData.Values);
                return Task.CompletedTask;
            }

            var actionDescriptor = _actionSelector.SelectBestCandidate(context, candidates);
            if (actionDescriptor == null)
            {
                //_logger.NoActionsMatched(context.RouteData.Values);
                return Task.CompletedTask;
            }

            context.Handler = (c) =>
            {
                var routeData = c.GetRouteData();

                var actionContext = new ActionContext(context.HttpContext, routeData, actionDescriptor);
                var invoker = _actionInvokerFactory.CreateInvoker(actionContext);
                if (invoker == null)
                {
                    throw new InvalidOperationException($"Could not invoke {actionDescriptor.DisplayName}");
                }

                return invoker.InvokeAsync();
            };

            return Task.CompletedTask;
        }
    }
}
