﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Categories.Models;
using Plato.Categories.Repositories;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;

namespace Plato.Categories.Stores
{
    
    public class EntityCategoryStore : IEntityCategoryStore<EntityCategory>
    {

        private const string ById = "ById";
        private const string ByEntityId = "ByEntityId";
        private const string ByEntityIdAndCategoryId = "ByEntityIdAndCategoryId";

        private readonly IEntityCategoryRepository<EntityCategory> _entityCategoryRepository;
        private readonly ILogger<EntityCategoryStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;

        public EntityCategoryStore(
            IEntityCategoryRepository<EntityCategory> entityCategoryRepository,
            ILogger<EntityCategoryStore> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _entityCategoryRepository = entityCategoryRepository;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;
        }
        
        public async Task<EntityCategory> CreateAsync(EntityCategory model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.CategoryId == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CategoryId));
            }
            
            if (model.EntityId == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }


            var result = await _entityCategoryRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens();
            }

            return result;
        }

        public async Task<EntityCategory> UpdateAsync(EntityCategory model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.CategoryId == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CategoryId));
            }

            if (model.EntityId == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }
            
            var result = await _entityCategoryRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens();
            }

            return result;
        }

        public async Task<bool> DeleteAsync(EntityCategory model)
        {
            var success = await _entityCategoryRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted category role for category '{0}' with id {1}",
                        model.CategoryId, model.Id);
                }
                CancelTokens();
            }

            return success;
        }

        public async Task<EntityCategory> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ById, id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _entityCategoryRepository.SelectByIdAsync(id));
        }

        public IQuery<EntityCategory> QueryAsync()
        {
            var query = new EntityCategoryQuery(this);
            return _dbQuery.ConfigureQuery<EntityCategory>(query); ;
        }

        public async Task<IPagedResults<EntityCategory>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityCategoryRepository.SelectAsync(dbParams));
        }

        public async Task<IEnumerable<EntityCategory>> GetByEntityIdAsync(int entityId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByEntityId, entityId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityCategoryRepository.SelectByEntityIdAsync(entityId));
        }

        public async Task<EntityCategory> GetByEntityIdAndCategoryIdAsync(int entityId, int categoryId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByEntityIdAndCategoryId, entityId, categoryId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityCategoryRepository.SelectByEntityIdAndCategoryIdAsync(entityId, categoryId));
        }

        public async Task<bool> DeleteByEntityIdAsync(int entityId)
        {
            var success = await _entityCategoryRepository.DeleteByEntityIdAsync(entityId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted all category roles for category '{0}'",
                        entityId);
                }
                CancelTokens();
            }

            return success;
        }

        public async Task<bool> DeleteByEntityIdAndCategoryIdAsync(int entityId, int categoryId)
        {

            var success = await _entityCategoryRepository.DeleteByEntityIdAndCategoryIdAsync(entityId, categoryId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted entity category for entity Id '{0}' and categoryId {1}",
                        entityId, entityId);
                }

                CancelTokens();
            }

            return success;
        }

        public void CancelTokens(EntityCategory model = null)
        {
            _cacheManager.CancelTokens(this.GetType());
        }

    }

}
