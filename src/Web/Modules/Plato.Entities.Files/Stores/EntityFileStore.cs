using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;
using Plato.Entities.Files.Models;
using Plato.Entities.Files.Repositories;

namespace Plato.Entities.Files.Stores
{

    public class EntityFileStore : IEntityFileStore<EntityFile>
    {

        private const string ByEntityId = "ByEntityId";

        private readonly IEntityFileRepository<EntityFile> _entityFileRepository;
        private readonly ILogger<EntityFileStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;

        public EntityFileStore(
            IEntityFileRepository<EntityFile> entityFileRepository,
            ILogger<EntityFileStore> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _entityFileRepository = entityFileRepository;
            _cacheManager = cacheManager;            
            _dbQuery = dbQuery;
            _logger = logger;
        }
        
        #region "Implementation"

        public async Task<EntityFile> CreateAsync(EntityFile model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.FileId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.FileId));
            }

            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }

            var result = await _entityFileRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;
        }

        public async Task<EntityFile> UpdateAsync(EntityFile model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.FileId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.FileId));
            }

            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }

            var result = await _entityFileRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;
        }

        public async Task<bool> DeleteAsync(EntityFile model)
        {
            var success = await _entityFileRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted Label role for Label '{0}' with id {1}",
                        model.FileId, model.Id);
                }

                CancelTokens(model);
            }

            return success;
        }

        public async Task<EntityFile> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _entityFileRepository.SelectByIdAsync(id));
        }

        public IQuery<EntityFile> QueryAsync()
        {
            var query = new EntityFileQuery(this);
            return _dbQuery.ConfigureQuery<EntityFile>(query); ;
        }

        public async Task<IPagedResults<EntityFile>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
                await _entityFileRepository.SelectAsync(dbParams));
        }

        public async Task<IEnumerable<EntityFile>> GetByEntityIdAsync(int entityId)
        {

            if (entityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entityId));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByEntityId, entityId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
                await _entityFileRepository.SelectByEntityIdAsync(entityId));
        }

        public async Task<bool> DeleteByEntityIdAsync(int entityId)
        {

            if (entityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entityId));
            }

            var success = await _entityFileRepository.DeleteByEntityIdAsync(entityId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted all files for entityId '{0}'",
                        entityId);
                }

                CancelTokens();
            }

            return success;
        }

        public async Task<bool> DeleteByFileIdAsync(int fileId)
        {

            if (fileId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fileId));
            }

            var success = await _entityFileRepository.DeleteByFileIdAsync(fileId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted all entity relationships for file id '{0}'",
                        fileId);
                }

                CancelTokens();
            }

            return success;
        }

        public async Task<bool> DeleteByEntityIdAndFileIdAsync(int entityId, int labelId)
        {

            var success = await _entityFileRepository.DeleteByEntityIdAndFileIdAsync(entityId, labelId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted entity files for entityId '{0}' and labelId {1}",
                        entityId, labelId);
                }

                CancelTokens();

            }

            return success;
        }

        public void CancelTokens(EntityFile model = null)
        {
            _cacheManager.CancelTokens(this.GetType());
        }

        #endregion

    }

}
