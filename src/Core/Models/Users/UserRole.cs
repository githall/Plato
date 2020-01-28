using System;
using System.Data;
using Microsoft.AspNetCore.Identity;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Abstractions;
using PlatoCore.Models.Roles;

namespace PlatoCore.Models.Users
{
    public class UserRole : IdentityUserRole<int>, IDbModel<UserRole>
    {
        public int Id { get; set; }

        public Role Role { get; private set; }

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("UserId"))
                UserId = Convert.ToInt32(dr["UserId"]);

            if (dr.ColumnIsNotNull("RoleId"))
                RoleId = Convert.ToInt32(dr["RoleId"]);

            if (RoleId > 0)
            {
                Role = new Role();
                Role.PopulateModel(dr);
            }

        }

    }
}