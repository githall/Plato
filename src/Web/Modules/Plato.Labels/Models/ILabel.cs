﻿using System;
using PlatoCore.Abstractions;
using PlatoCore.Models;

namespace Plato.Labels.Models
{

    public interface ILabel :
        IMetaData<LabelData>,
        ILabelBase,
        IDbModel
    {

        int ParentId { get; set; }

        int FeatureId { get; set; }
        
        int SortOrder { get; set; }

        int TotalEntities { get; set; }

        int TotalFollows { get; set; }

        int TotalViews { get; set; }
        
        int LastEntityId { get; set; }

        DateTimeOffset? LastEntityDate { get; set; }
        
        int CreatedUserId { get; set; }

        DateTimeOffset? CreatedDate { get; set; }

        int ModifiedUserId { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }
        
    }

}
