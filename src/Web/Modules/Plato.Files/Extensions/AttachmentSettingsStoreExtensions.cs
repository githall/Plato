using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Files.Models;
using Plato.Files.Stores;
using PlatoCore.Abstractions.Extensions;

namespace Plato.Files.Extensions
{
    public static class AttachmentSettingsStoreExtensions
    {

        public static async Task<FileSetting> GetByRoleIdAsync(this IFileSettingsStore<FileSettings> store, int roleId)
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

        public static async Task<FileSettings> GetByRoleIdsAsync(this IFileSettingsStore<FileSettings> store, int[] roleIds)
        {

            if (roleIds == null)
            {
                throw new ArgumentNullException(nameof(roleIds));
            }

            var output = new List<FileSetting>();
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

            return new FileSettings()
            {
                Settings = output
            };

        }

        public static async Task<FileSettings> SaveAsync(this IFileSettingsStore<FileSettings> store, FileSetting model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.RoleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.RoleId));
            }

            var output = new List<FileSetting>();
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

            return await store.SaveAsync(new FileSettings()
            {
                Settings = output
            });

        }

    }

}
