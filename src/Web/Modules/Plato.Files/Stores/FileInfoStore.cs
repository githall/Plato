using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Files.Models;
using Plato.Files.Repositories;
using PlatoCore.Cache.Abstractions;

namespace Plato.Files.Stores
{

    public class FileInfoStore : IFileInfoStore<FileInfo>
    {

        private readonly IFileInfoRepository<FileInfo> _fileInfoRepository;
        private readonly ILogger<FileInfoStore> _logger;      
        private readonly ICacheManager _cacheManager;

        public FileInfoStore(
            IFileInfoRepository<FileInfo> fileInfoRepository,
            ILogger<FileInfoStore> logger,
            ICacheManager cacheManager)
        {
            _fileInfoRepository = fileInfoRepository;
            _cacheManager = cacheManager;
            _logger = logger;       
        }

        public async Task<FileInfo> GetByUserIdAsync(int userId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), userId);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _fileInfoRepository.SelectByUserIdAsync(userId));
        }

        public void CancelTokens(FileInfo model)
        {
            _cacheManager.CancelTokens(this.GetType());
        }

    }

}
