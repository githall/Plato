using System;
using System.Data;
using PlatoCore.Abstractions.Extensions;
using Plato.Files.Models;

namespace Plato.Entities.Files.Models
{

    public class EntityFile : File
    {

        public int EntityId { get; set; }

        public int FileId { get; set; }

        public override void PopulateModel(IDataReader dr)
        {

            base.PopulateModel(dr);

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("EntityId"))
                EntityId = Convert.ToInt32(dr["EntityId"]);

            if (dr.ColumnIsNotNull("FileId"))
                FileId = Convert.ToInt32(dr["FileId"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = (DateTimeOffset)dr["ModifiedDate"];

        }

    }

}
