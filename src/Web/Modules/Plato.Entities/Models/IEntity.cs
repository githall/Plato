using System;
using System.Data;
using System.Threading.Tasks;
using PlatoCore.Abstractions;
using PlatoCore.Models.Users;

namespace Plato.Entities.Models
{

    public interface IEntity : ISimpleEntity,
        IEntityMetaData<IEntityData>
    {

        string Message { get; set; }

        string Html { get; set; }

        string Abstract { get; set; }

        string Urls { get; set; }

        int TotalViews { get; set; }

        int TotalReplies { get; set; }

        int TotalAnswers { get; set; }
        
        int TotalParticipants { get; set; }

        int TotalReactions { get; set; }

        int TotalFollows { get; set; }

        int TotalReports { get; set; }

        int TotalStars { get; set; }

        int TotalRatings { get; set; }

        int SummedRating { get; set; }
        
        int MeanRating { get; set; }

        int TotalLinks { get; set; }

        int TotalImages { get; set; }

        int TotalWords { get; set; }
              
        string IpV4Address { get; set; }

        string IpV6Address { get; set; }

        int CreatedUserId { get; set; }

        DateTimeOffset? CreatedDate { get; set; }

        int EditedUserId { get; set; }

        DateTimeOffset? EditedDate { get; set; }
        
        int ModifiedUserId { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }

        int LastReplyId { get; set; }

        int LastReplyUserId { get; set; }
        
        DateTimeOffset? LastReplyDate { get; set; }
        
        SimpleUser CreatedBy { get; }

        SimpleUser ModifiedBy { get; }

        SimpleUser LastReplyBy { get; }
        
        Task<EntityUris> GetEntityUrlsAsync();       

    }

    public interface IEntityReplyData
    {

        int Id { get; set; }

        int ReplyId { get; set; }

        string Key { get; set; }

        string Value { get; set; }

        DateTimeOffset? CreatedDate { get; set; }

        int CreatedUserId { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }

        int ModifiedUserId { get; set; }

    }

    public interface IEntityData
    {

        int Id { get; set; }

        int EntityId { get; set; }

        string Key { get; set; }

        string Value { get; set; }

        DateTimeOffset? CreatedDate { get; set; }

        int CreatedUserId { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }

        int ModifiedUserId { get; set; }

    }

}

