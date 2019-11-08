using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Internal.Models.Roles;
using System.Security.Principal;
using System.Collections.Concurrent;

namespace Plato.Roles.Services
{
    public class RolesPermissionsHandler : AuthorizationHandler<PermissionRequirement>
    {

        private readonly IPlatoRoleStore _platoRoleStore;
        private readonly IContextFacade _contextFacade;

        public RolesPermissionsHandler(             
            IPlatoRoleStore platoRoleStore,
            IContextFacade contextFacade)
        {            
            _platoRoleStore = platoRoleStore;
            _contextFacade = contextFacade;
        }

        #region "Implementation"

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {

            if (context.HasSucceeded)
            {
                // This handler is not revoking any pre-existing grants.
                return;
            }

            // Determine which set of permissions would satisfy the access check
            var grantingNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            PermissionNames(requirement.Permission, grantingNames);

            // Get roles to examine
            var rolesToExamine = await GetRolesToExamine(context.User.Identity);

            // Iterate roles checking claims for each role
            foreach (var role in rolesToExamine)
            {

                foreach (var claim in role.RoleClaims)
                {

                    // Simple case-sensitive comparison for performance 
                    if (claim.ClaimType != Permission.ClaimTypeName)
                    {
                        continue;
                    }

                    var permissionName = claim.ClaimValue;
                    if (grantingNames.Contains(permissionName))
                    {
                        context.Succeed(requirement);
                        return;
                    }

                }

            }

        }

        #endregion

        #region "Private Methods"

        private ConcurrentDictionary<int, IList<Role>> _scopedUserRoles
            = new ConcurrentDictionary<int, IList<Role>>();

        async Task<IList<Role>> GetRolesToExamine(IIdentity identity)
        {

            var user = await _contextFacade.GetAuthenticatedUserAsync(identity);

            // Are the roles available in our scoped cache?
            if (_scopedUserRoles.ContainsKey(user?.Id ?? 0))
            {
                return _scopedUserRoles[user?.Id ?? 0];
            }

            // Add the roles to our scoped cache
            if (user != null)
            {
                if (!_scopedUserRoles.ContainsKey(user.Id))
                {                    
                    _scopedUserRoles.TryAdd(user.Id, await _platoRoleStore.GetRolesByUserIdAsync(user.Id));
                }                
            }
            else
            {
                if (!_scopedUserRoles.ContainsKey(0))
                {
                    _scopedUserRoles.TryAdd(0, new List<Role>() {
                        await _platoRoleStore.GetByNameAsync(DefaultRoles.Anonymous)
                    });
                }                
            }

            return _scopedUserRoles[user?.Id ?? 0];

        }

        static void PermissionNames(
            IPermission permission, 
            HashSet<string> stack)
        {
            // The given name is tested
            stack.Add(permission.Name);

            // Iterate implied permissions to grant, it present
            if (permission.ImpliedBy != null)
            {
                foreach (var impliedBy in permission.ImpliedBy)
                {
                    // Avoid potential recursion
                    if (stack.Contains(impliedBy.Name))
                    {
                        continue;
                    }

                    // Otherwise accumulate the implied permission names recursively
                    PermissionNames(impliedBy, stack);
                }
            }

        }

        #endregion

    }

}
