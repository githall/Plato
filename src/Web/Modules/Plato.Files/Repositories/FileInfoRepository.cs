using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Files.Models;
using PlatoCore.Data.Abstractions;

namespace Plato.Files.Repositories
{

    public class FileInfoRepository : IFileInfoRepository<FileInfo> 
    {

        private readonly ILogger<FileInfoRepository> _logger;
        private readonly IDbContext _dbContext;

        public FileInfoRepository(
            ILogger<FileInfoRepository> logger,
            IDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<FileInfo> SelectByUserIdAsync(int userId)
        {
            FileInfo info = null;
            using (var context = _dbContext)
            {
                info = await context.ExecuteReaderAsync<FileInfo>(
                    CommandType.StoredProcedure,
                    "SelectFileInfoByUserId",
                    async reader =>
                    {
                        if ((reader != null) && reader.HasRows)
                        {
                            await reader.ReadAsync();
                            info = new FileInfo();
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
