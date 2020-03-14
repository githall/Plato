﻿using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Models.Roles;
using PlatoCore.Models.Users;
using PlatoCore.Stores.Abstractions.Users;

namespace PlatoCore.Stores.Users
{
    
    public class UserRoleDecorator : IUserRoleDecorator
    {

        private readonly IPlatoUserRoleStore<UserRole> _userRoleStore;

        public UserRoleDecorator(
            IPlatoUserRoleStore<UserRole> userRoleStore)
        {
            _userRoleStore = userRoleStore;
        }

        public async Task<IEnumerable<User>> DecorateAsync(IEnumerable<User> users)
        {

            if (users == null)
            {
                return null;
            }

            // Get all user roles matching supplied users
            var userRoles = await _userRoleStore.QueryAsync()
                .Take(int.MaxValue, false)
                .Select<UserRoleQueryParams>(q =>
                {
                    q.UserId.IsIn(users.Select(u => u.Id).ToArray());
                }).ToList();
            
            // No roles to decorate
            if (userRoles == null)
            {
                return users;
            }

            // Hot code path - avoid linq queries
            var output = users.ToList();

            // Reduce iterations and allow index access
            var kvp = new ConcurrentDictionary<int, IList<Role>>();
            foreach (var userRole in userRoles.Data)
            {
                kvp.AddOrUpdate(userRole.UserId, new List<Role>()
                {
                    userRole.Role
                }, (k, v) =>
                {
                    v.Add(userRole.Role);
                    return v;
                });
            }

            // Decorate the user object with role data
            foreach (var user in output)
            {
                if (kvp.ContainsKey(user.Id))
                {
                    user.UserRoles = kvp[user.Id];
                    user.RoleNames = kvp[user.Id].Select(r => r.Name).ToList();
                }
            }

            // Clean up
            kvp.Clear();

            // Return decorated user
            return output;

        }

        public async Task<User> DecorateAsync(User user)
        {
            // Get all user roles matching supplied user
            var roles = await _userRoleStore.QueryAsync()
                .Select<UserRoleQueryParams>(q => { q.UserId.Equals(user.Id); })
                .ToList();

            if (user == null || roles?.Data == null)
            {
                return null;
            }
            
            user.UserRoles = roles.Data.Select(ur => ur.Role).ToList();
            user.RoleNames = user.UserRoles.Select(r => r.Name).ToList();
            
            return user;

        }
        
    }

}
