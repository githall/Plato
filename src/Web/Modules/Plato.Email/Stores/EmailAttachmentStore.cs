using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Email.Repositories;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Stores.Abstractions.FederatedQueries;
using PlatoCore.Stores.Abstractions.QueryAdapters;

namespace Plato.Email.Stores
{

    public class EmailAttachmentStore : IEmailAttachmentStore<EmailAttachment>
    {

        private const string ById = "ById";

        private readonly IEmailAttachmentRepository<EmailAttachment> _attachmentRepository;
        private readonly IFederatedQueryManager<EmailAttachment> _federatedQueryManager;
        private readonly IQueryAdapterManager<EmailAttachment> _queryAdapterManager;        
        private readonly ILogger<EmailAttachmentStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;        

        public EmailAttachmentStore(
            IEmailAttachmentRepository<EmailAttachment> attachmentRepository,
            IFederatedQueryManager<EmailAttachment> federatedQueryManager,            
            IQueryAdapterManager<EmailAttachment> queryAdapterManager,
            ILogger<EmailAttachmentStore> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _federatedQueryManager = federatedQueryManager;
            _attachmentRepository = attachmentRepository;
            _queryAdapterManager = queryAdapterManager;
            _cacheManager = cacheManager;              
            _dbQuery = dbQuery;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<EmailAttachment> CreateAsync(EmailAttachment model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.EmailId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EmailId));
            }

            var result = await _attachmentRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<EmailAttachment> UpdateAsync(EmailAttachment model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.EmailId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EmailId));
            }

            var result = await _attachmentRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<bool> DeleteAsync(EmailAttachment model)
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
                    _logger.LogInformation("Deleted email attachment '{0}' with id {1}",
                        model.Name, model.Id);
                }

                CancelTokens(model);

            }

            return success;

        }

        public async Task<EmailAttachment> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ById, id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _attachmentRepository.SelectByIdAsync(id));
        }

        public IQuery<EmailAttachment> QueryAsync()
        {
            return _dbQuery.ConfigureQuery(new EmailAttachmentQuery(this)
            {
                FederatedQueryManager = _federatedQueryManager,
                QueryAdapterManager = _queryAdapterManager
            });
        }

        public async Task<IPagedResults<EmailAttachment>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting email attachments for key '{0}' with the following parameters: {1}",
                            token.ToString(), dbParams.Select(p => p.Value));
                }

                return await _attachmentRepository.SelectAsync(dbParams);
                
            });
        }

        public void CancelTokens(EmailAttachment model = null)
        {

            _cacheManager.CancelTokens(this.GetType());

            _cacheManager.CancelTokens(typeof(EmailStore));

            if (model != null)
            {
                _cacheManager.CancelTokens(this.GetType(), ById, model.Id);
            }

        }

        #endregion

    }

}
