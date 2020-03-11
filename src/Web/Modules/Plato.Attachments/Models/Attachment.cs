using System;
using System.Data;
using PlatoCore.Abstractions;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Models.Users;

namespace Plato.Attachments.Models
{

    public class Attachment : IDbModel
    {

        public int Id { get; set; }

        public int FeatureId { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public string Extension { get; set; }

        public byte[] ContentBlob { get; set; }

        public string ContentType { get; set; }

        public long ContentLength { get; set; }

        public string ContentGuid { get; set; }

        public string ContentCheckSum { get; set; }

        public int TotalViews { get; set; }

        public string ModuleId { get; private set; }

        public int CreatedUserId { get; set; }

        public ISimpleUser CreatedBy { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public int ModifiedUserId { get; set; }

        public DateTimeOffset? ModifiedDate { get; set; }

        public ISimpleUser ModifiedBy { get; set; }

        public virtual void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("FeatureId"))
                FeatureId = Convert.ToInt32(dr["FeatureId"]);

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

            if (dr.ColumnIsNotNull("ContentGuid"))
                ContentGuid = Convert.ToString(dr["ContentGuid"]);

            if (dr.ColumnIsNotNull("ContentCheckSum"))
                ContentCheckSum = Convert.ToString(dr["ContentCheckSum"]);

            if (dr.ColumnIsNotNull("TotalViews"))
                TotalViews = Convert.ToInt32(dr["TotalViews"]);

            if (dr.ColumnIsNotNull("ModuleId"))
                ModuleId = Convert.ToString(dr["ModuleId"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (CreatedUserId > 0)
            {
                CreatedBy = new SimpleUser
                {
                    Id = CreatedUserId
                };
                if (dr.ColumnIsNotNull("CreatedUserName"))
                    CreatedBy.UserName = Convert.ToString(dr["CreatedUserName"]);
                if (dr.ColumnIsNotNull("CreatedDisplayName"))
                    CreatedBy.DisplayName = Convert.ToString(dr["CreatedDisplayName"]);
                if (dr.ColumnIsNotNull("CreatedAlias"))
                    CreatedBy.Alias = Convert.ToString(dr["CreatedAlias"]);
                if (dr.ColumnIsNotNull("CreatedPhotoUrl"))
                    CreatedBy.PhotoUrl = Convert.ToString(dr["CreatedPhotoUrl"]);
                if (dr.ColumnIsNotNull("CreatedPhotoColor"))
                    CreatedBy.PhotoColor = Convert.ToString(dr["CreatedPhotoColor"]);
                if (dr.ColumnIsNotNull("CreatedSignatureHtml"))
                    CreatedBy.SignatureHtml = Convert.ToString(dr["CreatedSignatureHtml"]);
                if (dr.ColumnIsNotNull("CreatedIsVerified"))
                    CreatedBy.IsVerified = Convert.ToBoolean(dr["CreatedIsVerified"]);
                if (dr.ColumnIsNotNull("CreatedIsStaff"))
                    CreatedBy.IsStaff = Convert.ToBoolean(dr["CreatedIsStaff"]);
                if (dr.ColumnIsNotNull("CreatedIsSpam"))
                    CreatedBy.IsSpam = Convert.ToBoolean(dr["CreatedIsSpam"]);
                if (dr.ColumnIsNotNull("CreatedIsBanned"))
                    CreatedBy.IsBanned = Convert.ToBoolean(dr["CreatedIsBanned"]);
            }

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);

            if (ModifiedUserId > 0)
            {
                ModifiedBy = new SimpleUser
                {
                    Id = ModifiedUserId
                };
                if (dr.ColumnIsNotNull("ModifiedUserName"))
                    ModifiedBy.UserName = Convert.ToString(dr["ModifiedUserName"]);
                if (dr.ColumnIsNotNull("ModifiedDisplayName"))
                    ModifiedBy.DisplayName = Convert.ToString(dr["ModifiedDisplayName"]);
                if (dr.ColumnIsNotNull("ModifiedAlias"))
                    ModifiedBy.Alias = Convert.ToString(dr["ModifiedAlias"]);
                if (dr.ColumnIsNotNull("ModifiedPhotoUrl"))
                    ModifiedBy.PhotoUrl = Convert.ToString(dr["ModifiedPhotoUrl"]);
                if (dr.ColumnIsNotNull("ModifiedPhotoColor"))
                    ModifiedBy.PhotoColor = Convert.ToString(dr["ModifiedPhotoColor"]);
                if (dr.ColumnIsNotNull("ModifiedSignatureHtml"))
                    ModifiedBy.SignatureHtml = Convert.ToString(dr["ModifiedSignatureHtml"]);
                if (dr.ColumnIsNotNull("ModifiedIsVerified"))
                    ModifiedBy.IsVerified = Convert.ToBoolean(dr["ModifiedIsVerified"]);
                if (dr.ColumnIsNotNull("ModifiedIsStaff"))
                    ModifiedBy.IsStaff = Convert.ToBoolean(dr["ModifiedIsStaff"]);
                if (dr.ColumnIsNotNull("ModifiedIsSpam"))
                    ModifiedBy.IsSpam = Convert.ToBoolean(dr["ModifiedIsSpam"]);
                if (dr.ColumnIsNotNull("ModifiedIsBanned"))
                    ModifiedBy.IsBanned = Convert.ToBoolean(dr["ModifiedIsBanned"]);
            }

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = (DateTimeOffset)dr["ModifiedDate"];

        }

    }

}