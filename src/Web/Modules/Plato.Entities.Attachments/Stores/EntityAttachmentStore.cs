using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;
using Plato.Entities.Attachments.Models;
using Plato.Entities.Attachments.Repositories;

namespace Plato.Entities.Attachments.Stores
{

    public class EntityAttachmentStore : IEntityAttachmentStore<EntityAttachment>
    {

        private const string ByEntityId = "ByEntityId";

        private readonly IEntityAttachmentRepository<EntityAttachment> _entityAttachmentRepository;
        private readonly ILogger<EntityAttachmentStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;

        public EntityAttachmentStore(
            IEntityAttachmentRepository<EntityAttachment> entityAttachmentRepository,
            ILogger<EntityAttachmentStore> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _entityAttachmentRepository = entityAttachmentRepository;
            _cacheManager = cacheManager;            
            _dbQuery = dbQuery;
            _logger = logger;
        }
        
        #region "Implementation"

        public async Task<EntityAttachment> CreateAsync(EntityAttachment model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.AttachmentId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.AttachmentId));
            }
            
            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }
            
            var result = await _entityAttachmentRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;
        }

        public async Task<EntityAttachment> UpdateAsync(EntityAttachment model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.AttachmentId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.AttachmentId));
            }

            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }

            var result = await _entityAttachmentRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;
        }

        public async Task<bool> DeleteAsync(EntityAttachment model)
        {
            var success = await _entityAttachmentRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted Label role for Label '{0}' with id {1}",
                        model.AttachmentId, model.Id);
                }

                CancelTokens(model);
            }

            return success;
        }

        public async Task<EntityAttachment> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _entityAttachmentRepository.SelectByIdAsync(id));
        }

        public IQuery<EntityAttachment> QueryAsync()
        {
            var query = new EntityAttachmentQuery(this);
            return _dbQuery.ConfigureQuery<EntityAttachment>(query); ;
        }

        public async Task<IPagedResults<EntityAttachment>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting entity labels for key '{0}' with the following parameters: {1}",
                        token.ToString(), dbParams.Select(p => p.Value));
                }

                return await _entityAttachmentRepository.SelectAsync(dbParams);

            });
        }

        public async Task<IEnumerable<EntityAttachment>> GetByEntityIdAsync(int entityId)
        {

            if (entityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entityId));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByEntityId, entityId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            await _entityAttachmentRepository.SelectByEntityIdAsync(entityId));
        }

        public async Task<bool> DeleteByEntityIdAsync(int entityId)
        {

            if (entityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entityId));
            }

            var success = await _entityAttachmentRepository.DeleteByEntityIdAsync(entityId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted all attachments for entityId '{0}'",
                        entityId);
                }

                CancelTokens();
            }

            return success;
        }

        public async Task<bool> DeleteByEntityIdAndLabelIdAsync(int entityId, int labelId)
        {

            var success = await _entityAttachmentRepository.DeleteByEntityIdAndAttachmentIdAsync(entityId, labelId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted entity attachment for entityId '{0}' and labelId {1}",
                        entityId, labelId);
                }

                CancelTokens();

            }

            return success;
        }

        public void CancelTokens(EntityAttachment model = null)
        {
            _cacheManager.CancelTokens(this.GetType());
        }

        #endregion

    }

}
