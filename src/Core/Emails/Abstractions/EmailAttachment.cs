using System;
using System.Data;
using PlatoCore.Abstractions;
using PlatoCore.Abstractions.Extensions;

namespace PlatoCore.Emails.Abstractions
{

    public class EmailAttachment : IDbModel
    {

        public int Id { get; set; }

        public int EmailId { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public string Extension { get; set; }

        public byte[] ContentBlob { get; set; }

        public string ContentType { get; set; }

        public long ContentLength { get; set; }
  
        public string ContentCheckSum { get; set; }


        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public virtual void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("EmailId"))
                EmailId = Convert.ToInt32(dr["EmailId"]);

            if (dr.ColumnIsNotNull("Name"))
                Name = Convert.ToString(dr["Name"]);

            if (dr.ColumnIsNotNull("Alias"))
                Alias = Convert.ToString(dr["Alias"]);

            if (dr.ColumnIsNotNull("Extension"))
                Extension = Convert.ToString(dr["Extension"]);

            if (dr.ColumnIsNotNull("ContentBlob"))
                ContentBlob = (byte[])(dr["ContentBlob"]);

            if (dr.ColumnIsNotNull("ContentType"))
                ContentType = Convert.ToString(dr["ContentType"]);

            if (dr.ColumnIsNotNull("ContentLength"))
                this.ContentLength = Convert.ToInt64(dr["ContentLength"]);

            if (dr.ColumnIsNotNull("ContentCheckSum"))
                ContentCheckSum = Convert.ToString(dr["ContentCheckSum"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

        }

    }

}
