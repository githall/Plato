using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PlatoCore.Models.Shell;

namespace PlatoCore.Shell.Abstractions
{
    public static class RunningShellTableExtensions
    {

        public static IShellSettings Match(
            this IRunningShellTable table,            
            HttpContext httpContext,
            ILogger logger)
        {

            var host = string.Empty;
            HttpRequest httpRequest = null;

            // use Host header to prevent proxy alteration of the original request
            try
            {

                httpRequest = httpContext.Request;
                if (httpRequest == null)
                {
                    return null;
                }

                host = httpRequest.Headers["Host"].ToString();
                return table.Match(host ?? string.Empty, httpRequest.Path);

            }
            catch (Exception e)
            {

                var url = host + httpRequest != null
                    ? httpRequest.Path.ToString()
                    : string.Empty;
                if (logger.IsEnabled(LogLevel.Error))
                {
                    logger.LogError(e, $"A problem occurred attempting to match the URL \"{url}\" to a specific tenant. Could not locate matching tenant!");
                }

                // can happen on cloud service for an unknown reason
                return null;

            }

        }

    }

}
