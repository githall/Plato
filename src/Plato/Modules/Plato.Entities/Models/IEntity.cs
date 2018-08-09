﻿using System;
using System.Collections.Generic;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Models
{

    public interface IEntity
    {

        int Id { get; set; }

        int FeatureId { get; set; }

        int CategoryId { get; set; }

        string Title { get; set; }

        string Alias { get; set; }
        
        string Message { get; set; }

        string Html { get; set; }

        string Abstract { get; set; }

        bool IsPrivate { get; set; }

        bool IsSpam { get; set; }

        bool IsPinned { get; set; }

        bool IsDeleted { get; set; }

        bool IsClosed { get; set; }
        
        int TotalViews { get; set; }

        int TotalReplies { get; set; }

        int TotalReactions { get; set; }

        int TotalFollows { get; set; }

        int TotalReports { get; set; }

        int MeanViews { get; set; }

        int MeanReplies { get; set; }

        int MeanReactions { get; set; }

        int MeanFollows { get; set; }

        int MeanReports { get; set; }

        int CreatedUserId { get; set; }

        DateTimeOffset CreatedDate { get; set; }

        int ModifiedUserId { get; set; }

        DateTimeOffset ModifiedDate { get; set; }

        SimpleUser CreatedBy { get; }

        SimpleUser ModifiedBy { get; } 
        
        IEnumerable<IEntityData> Data { get; set; }

        IDictionary<Type, ISerializable> MetaData { get; }

        void AddOrUpdate<T>(T obj) where T : class;

        void AddOrUpdate(Type type, ISerializable obj);

        T GetOrCreate<T>() where T : class;

        void PopulateModel(IDataReader dr);

    }
    
    public interface IEntityData
    {

        int Id { get; set; }

        int EntityId { get; set; }

        string Key { get; set; }

        string Value { get; set; }

        DateTime? CreatedDate { get; set; }

        int CreatedUserId { get; set; }

        DateTime? ModifiedDate { get; set; }

        int ModifiedUserId { get; set; }

    }

}
