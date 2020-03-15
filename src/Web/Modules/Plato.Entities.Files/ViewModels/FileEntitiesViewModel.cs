using Plato.Entities.Models;
using PlatoCore.Data.Abstractions;

namespace Plato.Entities.Files.ViewModels
{
    public class FileEntitiesViewModel
    {

        public IPagedResults<Entity> Results { get; set; }

    }
}
