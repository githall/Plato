using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Plato.Entities.Extensions;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using PlatoCore.Abstractions;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Modules.Abstractions;
using PlatoCore.Stores.Abstractions.FederatedQueries;
using PlatoCore.Stores.Abstractions.QueryAdapters;

namespace Plato.Entities.Stores
{

    public class SimpleEntityStore<TEntity> : ISimpleEntityStore<TEntity> where TEntity : class, ISimpleEntity
    {

        public const string  ById = "ById";
        public const string ByFeatureId = "ByFeatureId";

        private readonly IFederatedQueryManager<TEntity> _federatedQueryManager;
        private readonly IQueryAdapterManager<TEntity> _queryAdapterManager;        
        private readonly ISimpleEntityRepository<TEntity> _entityRepository;
        private readonly ITypedModuleProvider _typedModuleProvider;
        private readonly ILogger<SimpleEntityStore<TEntity>> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;
        
        public SimpleEntityStore(
            IQueryAdapterManager<TEntity> queryAdapterManager,
            IFederatedQueryManager<TEntity> federatedQueryManager,
            ISimpleEntityRepository<TEntity> entityRepository,
            ITypedModuleProvider typedModuleProvider,
            ILogger<SimpleEntityStore<TEntity>> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _federatedQueryManager = federatedQueryManager;
            _queryAdapterManager = queryAdapterManager;
            _typedModuleProvider = typedModuleProvider;
            _entityRepository = entityRepository;       
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;
        }

        #region "Implementation"

        public IQuery<TEntity> QueryAsync()
        {
            return _dbQuery.ConfigureQuery(new SimpleEntityQuery<TEntity>(this)
            {
                FederatedQueryManager = _federatedQueryManager,
                QueryAdapterManager = _queryAdapterManager
            });
        }

        public async Task<IPagedResults<TEntity>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var results = await _entityRepository.SelectAsync(dbParams);               
                return results;
            });

        }

        public async Task<IEnumerable<TEntity>> GetByFeatureIdAsync(int featureId)
        {

            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByFeatureId, featureId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var results = await _entityRepository.SelectByFeatureIdAsync(featureId);
                if (results != null)
                {             
                    results = results.BuildHierarchy<TEntity>();
                    results = results.OrderBy(e => e.SortOrder);
                }

                return results;

            });

        }

        public async Task<IEnumerable<TEntity>> GetParentsByIdAsync(int entityId)
        {

            if (entityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entityId));
            }

            var entity = await GetByIdAsync(entityId);
            if (entity == null)
            {
                return null;
            }

            if (entity.FeatureId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entity.FeatureId));
            }

            var entities = await GetByFeatureIdAsync(entity.FeatureId);
            return entities?.RecurseParents<TEntity>(entity.Id).Reverse();

        }

        public async Task<IEnumerable<TEntity>> GetChildrenByIdAsync(int entityId)
        {

            var entity = await GetByIdAsync(entityId);
            if (entity == null)
            {
                return null;
            }

            var entities = await GetByFeatureIdAsync(entity.FeatureId);
            return entities?.RecurseChildren<TEntity>(entity.Id).Reverse();
        }
        
        public void CancelTokens(TEntity model)
        {
            CancelTokensInternal(model);
        }

        #endregion

        #region "Private Methods"

        void CancelTokensInternal(TEntity model)
        {

            // Clear cache for current type, EntityStore<Entity>,
            // EntityStore<Topic>, EntityStore<Article> etc
            _cacheManager.CancelTokens(this.GetType());

            // If we instantiate the EntityStore via a derived type
            // of IEntity i.e. EntityStore<SomeEntity> ensures we clear
            // the cache for the base entity store. We don't want our
            // base entity cache polluting our derived type cache
            if (this.GetType() != typeof(EntityStore<Entity>))
            {
                _cacheManager.CancelTokens(typeof(EntityStore<Entity>));
            }

            // Clear entity data
            _cacheManager.CancelTokens(typeof(EntityDataStore));

        }

        public Task<TEntity> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}
