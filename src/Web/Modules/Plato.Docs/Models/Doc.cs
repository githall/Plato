using System.Collections.Generic;
using Plato.Entities.Models;

namespace Plato.Docs.Models
{

    public class Doc : Entity
    {
        public bool IsNew { get; set; }   

        public IEnumerable<ISimpleEntity> ChildEntities { get; set; }

    }

}
