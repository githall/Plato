using System;
using System.Data;
using PlatoCore.Abstractions;
using PlatoCore.Abstractions.Extensions;

namespace Plato.Tenants.SignUp.Models
{
    public class SignUp : IDbModel
    {

        public int Id { get; set; }

        public string SessionId { get; set; }

        public string Email { get; set; }

        public string CompanyName { get; set; }

        public string CompanyNameAlias { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool EmailUpdates { get; set; }

        public string SecurityToken { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("SessionId"))
                SessionId = Convert.ToString(dr["SessionId"]);

            if (dr.ColumnIsNotNull("Email"))
                Email = Convert.ToString(dr["Email"]);

            if (dr.ColumnIsNotNull("CompanyName"))
                CompanyName = Convert.ToString((dr["CompanyName"]));

            if (dr.ColumnIsNotNull("CompanyNameAlias"))
                CompanyNameAlias = Convert.ToString((dr["CompanyNameAlias"]));

            if (dr.ColumnIsNotNull("EmailConfirmed"))
                EmailConfirmed = Convert.ToBoolean(dr["EmailConfirmed"]);

            if (dr.ColumnIsNotNull("EmailUpdates"))
                EmailUpdates = Convert.ToBoolean(dr["EmailUpdates"]);

            if (dr.ColumnIsNotNull("SecurityToken"))
                SecurityToken = Convert.ToString((dr["SecurityToken"]));

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

        }

    }
}
