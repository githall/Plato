using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Attachments.Models;
using Plato.Attachments.Stores;
using PlatoCore.Abstractions.Extensions;

namespace Plato.Attachments.Extensions
{
    public static class AttachmentSettingsStoreExtensions
    {

        public static async Task<AttachmentSetting> GetByRoleIdAsync(this IAttachmentSettingsStore<AttachmentSettings> store, int roleId)
        {

            if (roleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(roleId));
            }

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

        public static async Task<AttachmentSettings> GetByRoleIdsAsync(this IAttachmentSettingsStore<AttachmentSettings> store, int[] roleIds)
        {

            if (roleIds == null)
            {
                throw new ArgumentNullException(nameof(roleIds));
            }

            var output = new List<AttachmentSetting>();
            var settings = await store.GetAsync();
            if (settings?.Settings != null)
            {
                foreach (var setting in settings.Settings)
                {
                    if (roleIds.Contains(setting.RoleId))
                    {
                        output.Add(setting);
                    }
                }
            }

            return new AttachmentSettings()
            {
                Settings = output
            };

        }

        public static async Task<AttachmentSettings> SaveAsync(this IAttachmentSettingsStore<AttachmentSettings> store, AttachmentSetting model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.RoleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.RoleId));
            }

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
