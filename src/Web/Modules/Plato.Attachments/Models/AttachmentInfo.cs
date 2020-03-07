using System;
using System.Data;
using PlatoCore.Abstractions;
using PlatoCore.Abstractions.Extensions;

namespace Plato.Attachments.Models
{

    public class AttachmentInfo : IDbModel
    {

        public int Count { get; set; }

        public long Length { get; set; }

        public virtual void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Count"))
                Count = Convert.ToInt32(dr["Count"]);

            if (dr.ColumnIsNotNull("Length"))
                Length = Convert.ToInt64(dr["Length"]);

        }

    }

}
