﻿using System;
using Microsoft.AspNetCore.Http;
using PlatoCore.Shell.Abstractions;
using PlatoCore.Models.Shell;

namespace PlatoCore.Shell.Abstractions
{
    public static class RunningShellTableExtensions
    {

        public static IShellSettings Match(
            this IRunningShellTable table,
            HttpContext httpContext)
        {
            // use Host header to prevent proxy alteration of the original request
            try
            {
                var httpRequest = httpContext.Request;
                if (httpRequest == null)
                {
                    return null;
                }

                var host = httpRequest.Headers["Host"].ToString();
                return table.Match(host ?? string.Empty, httpRequest.Path);
            }
            catch (Exception)
            {
                // can happen on cloud service for an unknown reason
                return null;
            }
        }

    }
}
