using System.Collections.Generic;
using Plato.Entities.Models;
using PlatoCore.Abstractions;

namespace Plato.Ideas.Models
{

    public class IdeaDetails : Serializable
    {
        public IEnumerable<EntityUser> LatestUsers { get; set; } = new List<EntityUser>();
    }
    
}
