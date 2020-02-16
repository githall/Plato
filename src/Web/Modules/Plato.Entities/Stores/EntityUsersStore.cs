using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Stores.Abstractions;

namespace Plato.Entities.Stores
{

    public interface IEntityUsersStore : IQueryableStore<EntityUser>
    {
    }

    public class EntityUsersStore : IEntityUsersStore
    {

        private readonly IEntityUsersRepository _entityUsersRepository;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;

        public EntityUsersStore(
            IEntityUsersRepository entityUsersRepository,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _entityUsersRepository = entityUsersRepository;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
        }


        public IQuery<EntityUser> QueryAsync()
        {
            var query = new EntityUserQuery(this);
            return _dbQuery.ConfigureQuery<EntityUser>(query); ;
        }

        public async Task<IPagedResults<EntityUser>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityUsersRepository.SelectAsync(dbParams));
        }

        public void CancelTokens(EntityUser model)
        {
            _cacheManager.CancelTokens(this.GetType());
        }

    }

}
