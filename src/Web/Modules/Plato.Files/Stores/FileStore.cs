using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Files.Repositories;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Stores.Abstractions.FederatedQueries;
using PlatoCore.Stores.Abstractions.QueryAdapters;
using PlatoCore.Text.Abstractions;

namespace Plato.Files.Stores
{

    public class FileStore : IFileStore<Models.File>
    {

        private const string ById = "ById";
        
        private readonly IFileRepository<Models.File> _attachmentRepository;
        private readonly IFederatedQueryManager<Models.File> _federatedQueryManager;
        private readonly IQueryAdapterManager<Models.File> _queryAdapterManager;
        private readonly ILogger<FileStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;
        private readonly IAliasCreator _aliasCreator;
       
        public FileStore(            
            IFederatedQueryManager<Models.File> federatedQueryManager,
            IFileRepository<Models.File> attachmentRepository,
            IQueryAdapterManager<Models.File> queryAdapterManager,
            ILogger<FileStore> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager,
            IAliasCreator aliasCreator)
        {
            _federatedQueryManager = federatedQueryManager;
            _attachmentRepository = attachmentRepository;
            _queryAdapterManager = queryAdapterManager;
            _cacheManager = cacheManager;
            _aliasCreator = aliasCreator;            
            _dbQuery = dbQuery;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<Models.File> CreateAsync(Models.File model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (string.IsNullOrEmpty(model.Alias))
            {
                model.Alias = _aliasCreator.Create(model.Name);
            }

            var result = await _attachmentRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<Models.File> UpdateAsync(Models.File model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (string.IsNullOrEmpty(model.Alias))
            {
                model.Alias = _aliasCreator.Create(model.Name);
            }

            var result = await _attachmentRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<bool> DeleteAsync(Models.File model)
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

        public async Task<Models.File> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ById, id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _attachmentRepository.SelectByIdAsync(id));
        }

        public IQuery<Models.File> QueryAsync()
        {

            return _dbQuery.ConfigureQuery(new FileQuery(this)
            {
                FederatedQueryManager = _federatedQueryManager,
                QueryAdapterManager = _queryAdapterManager
            });
            var query = new FileQuery(this);
            return _dbQuery.ConfigureQuery<Models.File>(query); ;
        }

        public async Task<IPagedResults<Models.File>> SelectAsync(IDbDataParameter[] dbParams)
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

        public void CancelTokens(Models.File model = null)
        {

            _cacheManager.CancelTokens(this.GetType());

            _cacheManager.CancelTokens(typeof(FileInfoStore));

            if (model != null)
            {
                _cacheManager.CancelTokens(this.GetType(), ById, model.Id);
            }

        }

        #endregion

    }

}
