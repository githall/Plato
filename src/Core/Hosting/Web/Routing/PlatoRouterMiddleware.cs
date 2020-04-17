using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Models.Shell;

namespace PlatoCore.Hosting.Web.Routing
{
    public class PlatoRouterMiddleware
    {

        private readonly Dictionary<string, RequestDelegate> _pipelines = new Dictionary<string, RequestDelegate>();
        private readonly RequestDelegate _next;        
        private readonly ILogger _logger;

        public PlatoRouterMiddleware(
            ILogger<PlatoRouterMiddleware> logger,
            RequestDelegate next)
        {           
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Begin Routing Request");
            }

            var shellSettings = (ShellSettings)context.Features[typeof(ShellSettings)];

            // Define a PathBase for the current request that is the RequestUrlPrefix.
            // This will allow any view to reference ~/ as the tenant's base URL.
            // Because IIS or another middle ware might have already set it, we just append the tenant prefix value.
            if (!String.IsNullOrEmpty(shellSettings.RequestedUrlPrefix))
            {
                //context.Request.PathBase += "/" + shellSettings.RequestedUrlPrefix;
                //context.Request.Path = context.Request.Path.ToString()
                //    .Substring(context.Request.PathBase.Value.Length);
            }

            // Do we need to rebuild the pipeline ?
            var rebuildPipeline = context.Items["BuildPipeline"] != null;
            lock (_pipelines)
            {
                if (rebuildPipeline && _pipelines.ContainsKey(shellSettings.Name))
                {
                    _pipelines.Remove(shellSettings.Name);
                }
            }

            RequestDelegate pipeline;

            // Building a pipeline can't be done by two requests
            lock (_pipelines)
            {
                if (!_pipelines.TryGetValue(shellSettings.Name, out pipeline))
                {
                    pipeline = BuildTenantPipeline(shellSettings, context);
                    if (shellSettings.State == TenantState.Running)
                    {
                        _pipelines.Add(shellSettings.Name, pipeline);
                    }
                }
            }

            await pipeline.Invoke(context);

        }

        // Build the middle ware pipeline for the current tenant
        public RequestDelegate BuildTenantPipeline(ShellSettings shellSettings, HttpContext httpContext)
        {

            var serviceProvider = httpContext.RequestServices;
            var startUps = serviceProvider.GetServices<IStartup>();
            var inlineConstraintResolver = serviceProvider.GetService<IInlineConstraintResolver>();
            var appBuilder = new ApplicationBuilder(serviceProvider);

            var routePrefix = string.Empty;
            if (!string.IsNullOrWhiteSpace(shellSettings.RequestedUrlPrefix))
            {
                routePrefix = shellSettings.RequestedUrlPrefix + "/";
            }

            // Create a default route builder using our PlatoRouter implementation 
            var routeBuilder = new RouteBuilder(appBuilder, new RouteHandler(_next))
            {
                DefaultHandler = serviceProvider.GetRequiredService<IPlatoRouter>()
            };

            // Build prefixed route builder
            var prefixedRouteBuilder = new PrefixedRouteBuilder(
                routePrefix,
                routeBuilder,
                inlineConstraintResolver);

            // Configure modules
            foreach (var startup in startUps)
            {
                startup.Configure(appBuilder, prefixedRouteBuilder, serviceProvider);
            }

            // Add the default template route to each shell 
            prefixedRouteBuilder.Routes.Add(new Route(
                prefixedRouteBuilder.DefaultHandler,
                "PlatoDefault",
                "{area:exists}/{controller}/{action}/{id?}",
                null,
                null,
                null,
                inlineConstraintResolver)
            );

            // Build router
            var router = prefixedRouteBuilder.Build();

            // Use router
            appBuilder.UseRouter(router);

            //appBuilder.UseEndpoints(routes =>
            //{
            //    foreach (var startup in startups)
            //    {
            //        startup.Configure(appBuilder, routes, ShellScope.Services);
            //    }
            //});


            // Configure captured HTTP context
            ConfigureCapturedHttpContext(httpContext, serviceProvider);

            // Configure captured router
            ConfigureCapturedRouter(httpContext, serviceProvider, router);

            // Build & return new pipeline
            var pipeline = appBuilder.Build();
            return pipeline;

        }

        private static void ConfigureCapturedHttpContext(HttpContext httpContext,  IServiceProvider serviceProvider)
        {
            // Create a captured HttpContext for use outside of application context
            var capturedHttpContext = serviceProvider.GetService<ICapturedHttpContext>();
            capturedHttpContext.Configure(state => state.Contextualize(httpContext));
        }

        private static void ConfigureCapturedRouter(HttpContext httpContext, IServiceProvider serviceProvider, IRouter router)
        {
            // Configure captured router for use outside of application context
            var capturedRouter = serviceProvider.GetService<ICapturedRouter>();
            capturedRouter.Configure(options =>
            {
                options.Router = router;
                options.BaseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}";
            });
        }

    }

}
