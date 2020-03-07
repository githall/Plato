using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Attachments.Models;
using Plato.Attachments.Repositories;
using PlatoCore.Cache.Abstractions;

namespace Plato.Attachments.Stores
{

    public class AttachmentInfoStore : IAttachmentInfoStore<AttachmentInfo>
    {

        private readonly IAttachmentInfoRepository<AttachmentInfo> _attachmentInfoRepository;
        private readonly ILogger<AttachmentInfoStore> _logger;      
        private readonly ICacheManager _cacheManager;

        public AttachmentInfoStore(
            IAttachmentInfoRepository<AttachmentInfo> attachmentInfoRepository,
            ILogger<AttachmentInfoStore> logger,
            ICacheManager cacheManager)
        {
            _attachmentInfoRepository = attachmentInfoRepository;
            _cacheManager = cacheManager;
            _logger = logger;       
        }

        public async Task<AttachmentInfo> GetByUserIdAsync(int userId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), userId);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _attachmentInfoRepository.SelectByUserIdAsync(userId));
        }

        public void CancelTokens(AttachmentInfo model)
        {
            _cacheManager.CancelTokens(this.GetType());
        }

    }

}
