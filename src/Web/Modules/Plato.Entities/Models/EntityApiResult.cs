using System.Runtime.Serialization;
using Plato.WebApi.Models;

namespace Plato.Entities.Models
{

    [DataContract]
    class EntityApiResult
    {

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "createdBy")]
        public UserApiResult CreatedBy { get; set; }

        [DataMember(Name = "modifiedBy")]
        public UserApiResult ModifiedBy { get; set; }

        [DataMember(Name = "lastReplyBy")]
        public UserApiResult LastReplyBy { get; set; }
        
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "createdDate")]
        public IFriendlyDate CreatedDate { get; set; }

    }
}
