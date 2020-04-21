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
using PlatoCore.Shell.Extensions;

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

        public async Task Invoke(HttpContext httpContext)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Begin Routing Request");
            }

            // Get shell from context set via PlatoContainerMiddleware
            var shellSettings = httpContext.GetShellSettings();

            // This will allow any view to reference ~/ as the tenant's base URL.
            // Because IIS or another middle ware might have already set it, we just append the tenant prefix value.
            if (!String.IsNullOrEmpty(shellSettings.RequestedUrlPrefix))
            {                
                httpContext.Request.PathBase += "/" + shellSettings.RequestedUrlPrefix;         
                if (httpContext.Request.Path.ToString().Length > httpContext.Request.PathBase.Value.Length)
                {
                    httpContext.Request.Path = httpContext.Request.Path.ToString().Substring(httpContext.Request.PathBase.Value.Length);
                }                
            }

            // Do we need to rebuild the pipeline ?
            var rebuildPipeline = httpContext.Items["BuildPipeline"] != null;
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
                    pipeline = BuildTenantPipeline(shellSettings, httpContext);
                    if (shellSettings.State == TenantState.Running)
                    {
                        _pipelines.Add(shellSettings.Name, pipeline);
                    }
                }
            }

            // Idea similar UsePathBaseMiddleware @ https://github.com/dotnet/aspnetcore/blob/425c196cba530b161b120a57af8f1dd513b96f67/src/Http/Http.Abstractions/src/Extensions/UsePathBaseMiddleware.cs
            if (httpContext.Request.Path.StartsWithSegments(httpContext.Request.PathBase, out var matchedPath, out var remainingPath))
            {          
                try
                {
                    await pipeline(httpContext);
                }
                finally
                {
                    httpContext.Request.Path = new PathString();
                    httpContext.Request.PathBase = new PathString();
                }
            }
            else
            {
                await pipeline(httpContext);
            }         

        }

        // Build the middle ware pipeline for the current tenant
        public RequestDelegate BuildTenantPipeline(IShellSettings shellSettings, HttpContext httpContext)
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
            var routeBuilder = new RouteBuilder(appBuilder, serviceProvider.GetRequiredService<IPlatoRouter>());

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

            // Configure captured HTTP context
            ConfigureCapturedHttpContext(httpContext, serviceProvider);

            // Configure captured router
            ConfigureCapturedRouter(httpContext, serviceProvider, router);

            // Build & return new pipeline          
            return appBuilder.Build();

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
