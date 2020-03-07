using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Attachments.Models;
using PlatoCore.Data.Abstractions;

namespace Plato.Attachments.Repositories
{

    public class AttachmentInfoRepository : IAttachmentInfoRepository<AttachmentInfo> 
    {

        private readonly ILogger<AttachmentInfoRepository> _logger;
        private readonly IDbContext _dbContext;

        public AttachmentInfoRepository(
            ILogger<AttachmentInfoRepository> logger,
            IDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<AttachmentInfo> SelectByUserIdAsync(int userId)
        {
            AttachmentInfo info = null;
            using (var context = _dbContext)
            {
                info = await context.ExecuteReaderAsync<AttachmentInfo>(
                    CommandType.StoredProcedure,
                    "SelectAttachmentInfoByUserId",
                    async reader =>
                    {
                        if ((reader != null) && reader.HasRows)
                        {
                            await reader.ReadAsync();
                            info = new AttachmentInfo();
                            info.PopulateModel(reader);
                        }

                        return info;
                    }, new IDbDataParameter[]
                    {
                        new DbParam("UserId", DbType.Int32, userId)
                    });

            }

            return info;

        }

    }

}
