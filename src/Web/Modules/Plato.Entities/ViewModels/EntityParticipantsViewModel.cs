using Plato.Entities.Models;
using PlatoCore.Data.Abstractions;

namespace Plato.Entities.ViewModels
{
    

    public class EntityParticipantsViewModel
    {

        public IPagedResults<EntityUser> Users { get; set; }
    }
}
