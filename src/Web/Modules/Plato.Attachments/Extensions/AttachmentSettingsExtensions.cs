using Plato.Attachments.Models;
using PlatoCore.Models.Users;
using System.Collections.Generic;

namespace Plato.Attachments.Extensions
{
    public static class AttachmentSettingsExtensions
    {

        public static string[] GetAllowedExtensions(this AttachmentSettings settings, User user)
        {

            if (user == null)
            {
                return null;
            }

            if (user.UserRoles == null)
            {
                return null;
            }

            List<string> output = null;
            var roles = user.UserRoles;
            foreach (var role in roles)
            {
                foreach (var setting in settings.Settings)
                {
                    if ((setting.RoleId == role.Id))
                    {                       
                        foreach (var extension in setting.AllowedExtensions)
                        {
                            if (output == null)
                            {
                                output = new List<string>();
                            }
                            if (!output.Contains(extension))
                            {
                                output.Add(extension);
                            }
                        }
                    }
                }
            }

            return output?.ToArray();

        }

    }

}
