using System;
using PlatoCore.Models.Shell;
using PlatoCore.Shell.Abstractions;

namespace PlatoCore.Models.Extensions
{
    public static class ShellSettingsExtensions
    {

        public static bool IsDefaultShell(this IShellSettings settings)
        {
            return ShellHelper.DefaultShellName.Equals(settings.Name, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetRequestedUrl(
            this IShellSettings settings,
            string scheme)
        {

            var host = settings.RequestedUrlHost ?? string.Empty;
            var prefix = settings.RequestedUrlPrefix ?? string.Empty;

            // Remove trailing / from host name
            if (host.EndsWith("/"))
            {
                host = host.Substring(host.Length - 1);
            }

            return !string.IsNullOrEmpty(host)
                ? $"{scheme}://{host}{prefix}"
                : "/" + prefix;

        }

    }

}
