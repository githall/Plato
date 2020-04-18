using System;
using Microsoft.AspNetCore.Http;
using PlatoCore.Models.Shell;

namespace PlatoCore.Shell.Extensions
{

    public static class ShellSettingsHttpContextExtensions
    {

        public static void SetShellSettings(this HttpContext context, IShellSettings settings)
        {

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            context.Features[typeof(ShellSettings)] = settings;

        }

        public static IShellSettings GetShellSettings(this HttpContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Features == null)
            {
                return null;
            }

            if (context.Features[typeof(ShellSettings)] == null)
            {
                return null;
            }

            return context.Features[typeof(ShellSettings)] as ShellSettings;
        }

    }

}
