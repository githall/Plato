using System;
using System.Collections.Generic;
using System.Data;
using PlatoCore.Abstractions;
using PlatoCore.Abstractions.Extensions;

namespace Plato.Entities.Models
{

    public interface ISimpleEntity :         
        IComparable<ISimpleEntity>,
        INestable<ISimpleEntity>,
        IDbModel
    {

        int Id { get; set; }

        int ParentId { get; set; }

        int FeatureId { get; set; }

        int CategoryId { get; set; }

        string Title { get; set; }

        string Alias { get; set; }

        bool IsHidden { get; set; }

        bool IsPrivate { get; set; }

        bool IsSpam { get; set; }

        bool IsPinned { get; set; }

        bool IsDeleted { get; set; }

        bool IsLocked { get; set; }

        bool IsClosed { get; set; }

        string ModuleId { get; set; }

        int Rank { get; set; }

        int MaxRank { get; set; }

        int Relevance { get; set; }

    }

    public class SimpleEntity : ISimpleEntity
    {

        // ISimpleEntity

        public int Id { get; set; }

        public int ParentId { get; set; }

        public int FeatureId { get; set; }

        public int CategoryId { get; set; }

        public string Title { get; set; }

        public string Alias { get; set; }

        public bool IsHidden { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsSpam { get; set; }

        public bool IsPinned { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsLocked { get; set; }

        public bool IsClosed { get; set; }

        public string ModuleId { get; set; }

        public int Rank { get; set; }

        public int MaxRank { get; set; }

        public int Relevance { get; set; }

        // INestable

        public IEnumerable<ISimpleEntity> Children { get; set; } = new List<ISimpleEntity>();

        public ISimpleEntity Parent { get; set; }

        public int Depth { get; set; }

        public int SortOrder { get; set; }

        // IDbModel

        public virtual void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("ParentId"))
                ParentId = Convert.ToInt32(dr["ParentId"]);

            if (dr.ColumnIsNotNull("FeatureId"))
                FeatureId = Convert.ToInt32(dr["FeatureId"]);

            if (dr.ColumnIsNotNull("CategoryId"))
                CategoryId = Convert.ToInt32(dr["CategoryId"]);

            if (dr.ColumnIsNotNull("Title"))
                Title = Convert.ToString(dr["Title"]);

            if (dr.ColumnIsNotNull("Alias"))
                Alias = Convert.ToString(dr["Alias"]);

            if (dr.ColumnIsNotNull("IsHidden"))
                IsHidden = Convert.ToBoolean(dr["IsHidden"]);

            if (dr.ColumnIsNotNull("IsPrivate"))
                IsPrivate = Convert.ToBoolean(dr["IsPrivate"]);

            if (dr.ColumnIsNotNull("IsSpam"))
                IsSpam = Convert.ToBoolean(dr["IsSpam"]);

            if (dr.ColumnIsNotNull("IsPinned"))
                IsPinned = Convert.ToBoolean(dr["IsPinned"]);

            if (dr.ColumnIsNotNull("IsDeleted"))
                IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);

            if (dr.ColumnIsNotNull("IsLocked"))
                IsLocked = Convert.ToBoolean(dr["IsLocked"]);

            if (dr.ColumnIsNotNull("IsClosed"))
                IsClosed = Convert.ToBoolean(dr["IsClosed"]);

            if (dr.ColumnIsNotNull("SortOrder"))
                SortOrder = Convert.ToInt32(dr["SortOrder"]);

            if (dr.ColumnIsNotNull("ModuleId"))
                ModuleId = Convert.ToString(dr["ModuleId"]);

            if (dr.ColumnIsNotNull("Rank"))
                Rank = Convert.ToInt32(dr["Rank"]);

            if (dr.ColumnIsNotNull("MaxRank"))
                MaxRank = Convert.ToInt32(dr["MaxRank"]);

            Relevance = Rank.ToPercentageOf(MaxRank);

        }

        // IComparable

        public int CompareTo(ISimpleEntity other)
        {

            if (other == null)
                return 1;
            var sortOrderCompare = other.SortOrder;
            if (this.SortOrder == sortOrderCompare)
                return 0;
            if (this.SortOrder < sortOrderCompare)
                return -1;
            if (this.SortOrder > sortOrderCompare)
                return 1;
            return 0;

        }

    }

}
