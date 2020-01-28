using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Repositories;

namespace Plato.Follows.Repositories
{
    public interface IFollowRepository<TModel> : IRepository<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> SelectByNameAndThingId(string name, int thingId);

        Task<TModel> SelectByNameThingIdAndCreatedUserId(string name,  int thingId, int userId);

    }

}
