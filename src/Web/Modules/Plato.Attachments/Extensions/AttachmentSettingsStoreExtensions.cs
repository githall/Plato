using Plato.Attachments.Models;
using Plato.Attachments.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Attachments.Extensions
{
    public static class AttachmentSettingsStoreExtensions
    {

        public static async Task<AttachmentSetting> GetByRoleIdAsync(this IAttachmentSettingsStore<AttachmentSettings> store, int roleId)
        {
            var settings = await store.GetAsync();
            if (settings?.Settings != null)
            {
                foreach (var setting in settings.Settings)
                {
                    if (setting.RoleId == roleId)
                    {
                        return setting;
                    }
                }
            }

            return null;

        }

        public static async Task<AttachmentSettings> SaveAsync(this IAttachmentSettingsStore<AttachmentSettings> store, AttachmentSetting model)
        {

            var output = new List<AttachmentSetting>();
            var settings = await store.GetAsync();          
            if (settings?.Settings != null)
            {              
                if (settings.Contains(model))
                {
                    foreach (var setting in settings.Settings)
                    {
                        if (setting.RoleId == model.RoleId)
                        {
                            output.Add(model);
                        }
                        else
                        {
                            output.Add(setting);
                        }
                    }
                }
                else
                {
                    foreach (var setting in settings.Settings)
                    {                     
                        output.Add(setting);                    
                    }
                    output.Add(model);
                }             
            } else
            {
                output.Add(model);
            }

            return await store.SaveAsync(new AttachmentSettings()
            {
                Settings = output
            });

        }

    }

}
