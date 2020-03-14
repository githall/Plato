using Plato.Entities.Models;
using PlatoCore.Data.Abstractions;

namespace Plato.Entities.Files.ViewModels
{
    public class FileEntitiesViewModel
    {

        IPagedResults<Entity> Results { get; set; }

    }
}
