﻿using System;
using System.Data;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
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

    public class EntityStore<TEntity> : IEntityStore<TEntity> where TEntity : class, IEntity
    {

        public const string  ById = "ById";
        public const string ByFeatureId = "ByFeatureId";

        private readonly IFederatedQueryManager<TEntity> _federatedQueryManager;
        private readonly IQueryAdapterManager<TEntity> _queryAdapterManager;
        private readonly IEntityDataStore<IEntityData> _entityDataStore;
        private readonly IEntityRepository<TEntity> _entityRepository;
        private readonly ITypedModuleProvider _typedModuleProvider;
        private readonly ILogger<EntityStore<TEntity>> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;
        
        public EntityStore(
            IQueryAdapterManager<TEntity> queryAdapterManager,
            IFederatedQueryManager<TEntity> federatedQueryManager,
            IEntityDataStore<IEntityData> entityDataStore,
            IEntityRepository<TEntity> entityRepository,
            ITypedModuleProvider typedModuleProvider,
            ILogger<EntityStore<TEntity>> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _federatedQueryManager = federatedQueryManager;
            _queryAdapterManager = queryAdapterManager;
            _typedModuleProvider = typedModuleProvider;
            _entityRepository = entityRepository;
            _entityDataStore = entityDataStore;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<TEntity> CreateAsync(TEntity model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Transform meta data
            model.Data = await SerializeMetaDataAsync(model);

            var result = await _entityRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return await MergeEntityData(result); ;
        }

        public async Task<TEntity> UpdateAsync(TEntity model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Transform meta data
            model.Data = await SerializeMetaDataAsync(model);

            var result = await _entityRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return await MergeEntityData(result);

        }

        public async Task<bool> DeleteAsync(TEntity model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var success = await _entityRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted entity with id {1}", model.Id);
                }

                CancelTokens(model);

            }

            return success;

        }

        public async Task<TEntity> GetByIdAsync(int id)
        {

            if (id <= 0)
            {
                throw new InvalidEnumArgumentException(nameof(id));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), ById, id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var entity = await _entityRepository.SelectByIdAsync(id);
                return await MergeEntityData(entity);
            });

        }

        public IQuery<TEntity> QueryAsync()
        {
            return _dbQuery.ConfigureQuery(new EntityQuery<TEntity>(this)
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
                if (results != null)
                {
                    results.Data = await MergeEntityData(results.Data);
                }
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
                    results = await MergeEntityData(results.ToList());
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

        async Task<IEnumerable<IEntityData>> SerializeMetaDataAsync(TEntity entity)
        {

            // Get all existing entity data
            var data = await _entityDataStore.GetByEntityIdAsync(entity.Id);

            // Prepare list to search, use dummy list if needed
            var dataList = data?.ToList() ?? new List<IEntityData>();

            // Iterate all meta data on the supplied type,
            // check if a key already exists, if so update existing key 
            var output = new List<IEntityData>();
            foreach (var item in entity.MetaData)
            {
                var key = item.Key.FullName;
                var entityData = dataList.FirstOrDefault(d => d.Key == key);
                if (entityData != null)
                {
                    entityData.Value = await item.Value.SerializeAsync();
                }
                else
                {
                    entityData = new EntityData()
                    {
                        Key = key,
                        Value = item.Value.Serialize()
                    };
                }

                output.Add(entityData);
            }

            return output;

        }

        async Task<IList<TEntity>> MergeEntityData(IList<TEntity> entities)
        {

            if (entities == null)
            {
                return null;
            }

            // Get all entity data matching supplied entity ids
            var results = await _entityDataStore.QueryAsync()
                .Take(int.MaxValue, false)
                .Select<EntityDataQueryParams>(q => { q.EntityId.IsIn(entities.Select(e => e.Id).ToArray()); })
                .ToList();

            if (results == null)
            {
                return entities;
            }

            // Merge data into entities
            return await MergeEntityData(entities, results.Data);

        }

        async Task<IList<TEntity>> MergeEntityData(IList<TEntity> entities, IList<IEntityData> data)
        {

            if (entities == null || data == null)
            {
                return entities;
            }

            for (var i = 0; i < entities.Count; i++)
            {
                entities[i].Data = data.Where(d => d.EntityId == entities[i].Id).ToList();
                entities[i] = await MergeEntityData(entities[i]);
            }

            return entities;

        }

        async Task<TEntity> MergeEntityData(TEntity entity)
        {

            if (entity == null)
            {
                return null;
            }

            if (entity.Data == null)
            {
                return entity;
            }

            foreach (var data in entity.Data)
            {
                var type = await GetModuleTypeCandidateAsync(data.Key);
                if (type != null)
                {
                    var obj = JsonConvert.DeserializeObject(data.Value, type);
                    entity.AddOrUpdate(type, (ISerializable)obj);
                }
            }

            return entity;

        }

        async Task<Type> GetModuleTypeCandidateAsync(string typeName)
        {
            return await _typedModuleProvider.GetTypeCandidateAsync(typeName, typeof(ISerializable));
        }

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

            // Clear simple entity store
            _cacheManager.CancelTokens(typeof(SimpleEntityStore<SimpleEntity>));

            // Clear entity data store
            _cacheManager.CancelTokens(typeof(EntityDataStore));

        }

        #endregion

    }

}
