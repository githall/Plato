using PlatoCore.Models.Shell;

namespace PlatoCore.Models.Extensions
{
    public static class ShellSettingsExtensions
    {

        public static string GetRequestedUrl(this IShellSettings settings)
        {

            var host = settings.RequestedUrlHost ?? string.Empty;
            var prefix = settings.RequestedUrlPrefix ?? string.Empty;

            // Remove trailing / from host name
            if (host.EndsWith("/"))
            {
                host = host.Substring(host.Length - 1);
            }

            return !string.IsNullOrEmpty(host)
                ? host + prefix
                : "/" + prefix;

        }

    }

}
