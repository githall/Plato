using System.Collections.Generic;
using Plato.Entities.Models;
using PlatoCore.Abstractions;

namespace Plato.Discuss.Models
{

    public class TopicDetails : Serializable
    {
        public IEnumerable<EntityUser> LatestUsers { get; set; } = new List<EntityUser>();
    }
    
}
