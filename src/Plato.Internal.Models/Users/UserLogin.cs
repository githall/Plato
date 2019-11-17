using System;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Models.Users
{

    public class UserLogin : UserLoginInfo, IDbModel
    {

        public UserLogin() : base(string.Empty, string.Empty, string.Empty)
        {

        }

        public UserLogin(UserLoginInfo info)
            : base(info.LoginProvider, info.ProviderKey, info.ProviderDisplayName)
        {

        }

        public UserLogin(string loginProvider, string providerKey, string displayName)
            : base(loginProvider, providerKey, displayName)
        {
        }

        public int Id { get; set; }

        public string UserId { get; set; }

        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }


        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("UserId"))
                UserId = Convert.ToString(dr["UserId"]);

            if (dr.ColumnIsNotNull("LoginProvider"))
                LoginProvider = Convert.ToString(dr["LoginProvider"]);

            if (dr.ColumnIsNotNull("ProviderKey"))
                ProviderKey = Convert.ToString(dr["ProviderKey"]);

            if (dr.ColumnIsNotNull("ProviderDisplayName"))
                ProviderDisplayName = Convert.ToString(dr["ProviderDisplayName"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

        }

    }

}
