using System;
using System.Data;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Abstractions;

namespace PlatoCore.Models.Badges
{
    public class UserBadge : IDbModel
    {

        public int Id { get; set; }

        public string BadgeName { get; set; }

        public int UserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
        
        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("BadgeName"))
                BadgeName = Convert.ToString(dr["BadgeName"]);

            if (dr.ColumnIsNotNull("UserId"))
                UserId = Convert.ToInt32(dr["UserId"]);
            
            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

        }

    }

}
