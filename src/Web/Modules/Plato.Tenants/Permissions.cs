using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Tenants
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ManageTenants =
            new Permission("ManageTenants", "Can manage tenants");
        
        public static readonly Permission AddTenants = 
            new Permission("AddTenants", "Can add new tenants");

        public static readonly Permission EditTenants =
            new Permission("EditTenants", "Can edit existing tenants");

        public static readonly Permission DeleteTenants =
            new Permission("DeleteTenants", "Can delete existing tenants");

        public static readonly Permission EditSettings =
            new Permission("EditTenantSettings", "Can edit default tenant settings");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageTenants,
                AddTenants,
                EditTenants,
                DeleteTenants,
                EditSettings
            };
        }

        public IEnumerable<DefaultPermissions<Permission>> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Administrator,
                    Permissions = new[]
                    {
                        ManageTenants,
                        AddTenants,
                        EditTenants,
                        DeleteTenants,
                        EditSettings
                    }
                }
            };
        }

    }

}
