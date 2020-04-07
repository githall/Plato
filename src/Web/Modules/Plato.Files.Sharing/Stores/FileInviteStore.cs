using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Plato.Files.Sharing.Models;
using Plato.Files.Sharing.Repositories;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Text.Abstractions;

namespace Plato.Files.Sharing.Stores
{

    public class FileInviteStore : IFileInviteStore<FileInvite>
    {
        
        private readonly IFileInviteRepository<FileInvite> _fileInviteRepository;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ILogger<FileInviteStore> _logger;
        private readonly ICacheManager _cacheManager;
        private readonly IKeyGenerator _keyGenerator;
        
        public FileInviteStore(
            IFileInviteRepository<FileInvite> fileInviteRepository,
            IDbQueryConfiguration dbQuery,
            ILogger<FileInviteStore> logger,
            ICacheManager cacheManager,
            IKeyGenerator keyGenerator)
        {
            _fileInviteRepository = fileInviteRepository;
            _keyGenerator = keyGenerator;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<FileInvite> CreateAsync(FileInvite model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.FileId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.FileId));
            }
            
            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }

            var result = await _fileInviteRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;
        }

        public async Task<FileInvite> UpdateAsync(FileInvite model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.FileId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.FileId));
            }

            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }

            var result = await _fileInviteRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(model);
            }

            return result;
        }

        public async Task<bool> DeleteAsync(FileInvite model)
        {
            
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }
            
            var success = await _fileInviteRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted file invite with EntityId {0} UserId {1}",
                        model.FileId, model.CreatedUserId);
                }

                CancelTokens(model);

            }

            return success;
        }

        public async Task<FileInvite> GetByIdAsync(int id)
        {
            return await _fileInviteRepository.SelectByIdAsync(id);
        }

        public IQuery<FileInvite> QueryAsync()
        {
            var query = new FileInviteQuery(this);
            return _dbQuery.ConfigureQuery<FileInvite>(query); ;
        }

        public async Task<IPagedResults<FileInvite>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting file invites for key '{0}' with parameters: {1}",
                        token.ToString(), dbParams.Select(p => p.Value));
                }

                return await _fileInviteRepository.SelectAsync(dbParams);

            });
        }

        public async Task<IEnumerable<FileInvite>> SelectByEmailAndFileIdAsync(string email, int fileId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), email, fileId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Adding followers for name {0} with thingId of {1} to cache with key {2}",
                        email, fileId, token.ToString());
                }

                return await _fileInviteRepository.SelectByEmailAndFileIdAsync(email, fileId);

            });
        }

        public async Task<FileInvite> SelectByEmailFileIdAndCreatedUserIdAsync(string email, int fileId, int createdUserId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), email, fileId, createdUserId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Adding follow details for name {0} with createdUserId of {1} and thingId of {2} to cache with key {3}",
                      email,  createdUserId, fileId, token.ToString());
                }

                return await _fileInviteRepository.SelectByEmailFileIdAndCreatedUserIdAsync(email, fileId, createdUserId);

            });
        }

        public void CancelTokens(FileInvite model = null)
        {
            _cacheManager.CancelTokens(this.GetType());
        }

        #endregion

    }

}
