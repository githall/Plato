using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Stores.Abstractions.FederatedQueries;
using PlatoCore.Stores.Abstractions.QueryAdapters;

namespace Plato.Entities.Stores
{

    public class FeatureEntityCountStore  : IFeatureEntityCountStore
    {

        private readonly IFederatedQueryManager<FeatureEntityCount> _federatedQueryManager;
        private readonly IQueryAdapterManager<FeatureEntityCount> _queryAdapterManager;
        private readonly IFeatureEntityCountRepository _featureEntityCountRepository;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;

        public FeatureEntityCountStore(
            IFeatureEntityCountRepository featureEntityCountRepository,
            IFederatedQueryManager<FeatureEntityCount> federatedQueryManager,
            IQueryAdapterManager<FeatureEntityCount> queryAdapterManager,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _featureEntityCountRepository = featureEntityCountRepository;
            _federatedQueryManager = federatedQueryManager;
            _queryAdapterManager = queryAdapterManager;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
        }

        public IQuery<FeatureEntityCount> QueryAsync()
        {
            return _dbQuery.ConfigureQuery(new FeatureEntityCountQuery<FeatureEntityCount>(this)
            {
                FederatedQueryManager = _federatedQueryManager,
                QueryAdapterManager = _queryAdapterManager
            });
        }

        public async Task<IPagedResults<FeatureEntityCount>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _featureEntityCountRepository.SelectAsync(dbParams));
        }

        public void CancelTokens(FeatureEntityCount model)
        {
            _cacheManager.CancelTokens(this.GetType());
        }

    }

}
