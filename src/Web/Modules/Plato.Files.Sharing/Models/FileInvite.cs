using System;
using System.Data;
using Newtonsoft.Json;
using PlatoCore.Abstractions;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Models.Users;

namespace Plato.Files.Sharing.Models
{
    public class FileInvite : IDbModel
    {

        public int Id { get; set; }

        public int FileId { get; set; }

        public string Email { get; set; }

        public int CreatedUserId { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        [JsonIgnore]
        public IUser CreatedBy { get; set; } = new User();

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("FileId"))
                FileId = Convert.ToInt32(dr["FileId"]);

            if (dr.ColumnIsNotNull("Email"))
                Email = Convert.ToString(dr["Email"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (CreatedUserId > 0)
            {
                CreatedBy.Id = CreatedUserId;
                if (dr.ColumnIsNotNull("UserName"))
                    CreatedBy.UserName = Convert.ToString(dr["UserName"]);
                if (dr.ColumnIsNotNull("Email"))
                    CreatedBy.Email = Convert.ToString(dr["Email"]);
                if (dr.ColumnIsNotNull("DisplayName"))
                    CreatedBy.DisplayName = Convert.ToString(dr["DisplayName"]);
            }

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

        }

    }

}
