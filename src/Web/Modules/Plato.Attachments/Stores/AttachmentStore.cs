using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Attachments.Repositories;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;

namespace Plato.Attachments.Stores
{

    public class AttachmentStore : IAttachmentStore<Models.Attachment>
    {

        private const string ById = "ById";

        private readonly IAttachmentRepository<Models.Attachment> _attachmentRepository;
        private readonly ILogger<AttachmentStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;
       
        public AttachmentStore(
            IAttachmentRepository<Models.Attachment> attachmentRepository,
            ILogger<AttachmentStore> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _attachmentRepository = attachmentRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }

        #region "Implementation"

        public async Task<Models.Attachment> CreateAsync(Models.Attachment model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            var result = await _attachmentRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<Models.Attachment> UpdateAsync(Models.Attachment model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            var result = await _attachmentRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<bool> DeleteAsync(Models.Attachment model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var success = await _attachmentRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted attachment '{0}' with id {1}",
                        model.Name, model.Id);
                }

                CancelTokens(model);

            }

            return success;

        }

        public async Task<bool> UpdateContentGuidAsync(int[] ids, string contentGuid)
        {

            var success = await _attachmentRepository.UpdateContentGuidAsync(ids, contentGuid);
            if (success)
            {
                CancelTokens();
            }

            return success;

        }

        public async Task<bool> UpdateContentGuidAsync(int id, string contentGuid)
        {

            var success = await _attachmentRepository.UpdateContentGuidAsync(id, contentGuid);
            if (success)
            {
                CancelTokens();
            }

            return success;

        }

        public async Task<Models.Attachment> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ById, id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _attachmentRepository.SelectByIdAsync(id));
        }

        public IQuery<Models.Attachment> QueryAsync()
        {
            var query = new AttachmentQuery(this);
            return _dbQuery.ConfigureQuery<Models.Attachment>(query); ;
        }

        public async Task<IPagedResults<Models.Attachment>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting attachments for key '{0}' with the following parameters: {1}",
                            token.ToString(), dbParams.Select(p => p.Value));
                }

                return await _attachmentRepository.SelectAsync(dbParams);
                
            });
        }

        public void CancelTokens(Models.Attachment model = null)
        {

            _cacheManager.CancelTokens(this.GetType());

            _cacheManager.CancelTokens(typeof(AttachmentInfoStore));

            if (model != null)
            {
                _cacheManager.CancelTokens(this.GetType(), ById, model.Id);
            }

        }

        #endregion

    }

}
