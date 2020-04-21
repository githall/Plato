using Microsoft.AspNetCore.Http;
using PlatoCore.Shell.Extensions;
using System;
using System.Threading.Tasks;

namespace PlatoCore.Hosting.Web.Middleware
{

    public class CleanPathBaseMiddleware
    {

        private readonly RequestDelegate _next;  

        public CleanPathBaseMiddleware(RequestDelegate next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            _next = next;    
        }

        public async Task Invoke(HttpContext httpContext)
        {

            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            PathString _pathBase;

            var shellSettings = httpContext.GetShellSettings();
            if (shellSettings != null)
            {
                if (!string.IsNullOrEmpty(shellSettings.RequestedUrlPrefix))
                {
                    _pathBase = new PathString("/" + shellSettings.RequestedUrlPrefix);
                }
            }

            if (_pathBase == null)
            {
                await _next(httpContext);
            }

 
            if (httpContext.Request.Path.StartsWithSegments(_pathBase, out var matchedPath, out var remainingPath))
            {
                var originalPath = httpContext.Request.Path;
                var originalPathBase = httpContext.Request.PathBase;
                httpContext.Request.Path = remainingPath;
                httpContext.Request.PathBase = originalPathBase.Add(matchedPath);

                try
                {
                    await _next(httpContext);
                }
                finally
                {
                    httpContext.Request.Path = originalPath;
                    httpContext.Request.PathBase = originalPathBase;
                }
            }
            else
            {
                await _next(httpContext);
            }

        }

    }

}
