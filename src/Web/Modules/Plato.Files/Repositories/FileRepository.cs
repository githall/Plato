using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;
using Plato.Files.Models;

namespace Plato.Files.Repositories
{

    public class FileRepository : IFileRepository<File>
    {

        private readonly ILogger<FileRepository> _logger;
        private readonly IDbContext _dbContext;

        public FileRepository(            
            ILogger<FileRepository> logger,
            IDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        
        #region "Implementation"

        public async Task<IPagedResults<File>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<File> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<File>>(
                    CommandType.StoredProcedure,
                    "SelectFilesPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<File>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new Models.File();
                                entity.PopulateModel(reader);
                                output.Data.Add(entity);
                            }

                            if (await reader.NextResultAsync())
                            {
                                await reader.ReadAsync();
                                output.PopulateTotal(reader);
                            }

                        }

                        return output;

                    },
                    dbParams);
             
            }

            return output;

        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting file with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteFileById",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;
        }

        public async Task<bool> UpdateContentGuidAsync(int[] ids, string contentGuid)
        {

            var success = true;
            foreach (var id in ids)
            {
                var result = await UpdateContentGuidAsync(id, contentGuid);
                if (!result)
                {
                    success = false;
                    break;
                }
            }

            return success;
        }

        public async Task<bool> UpdateContentGuidAsync(int id, string contentGuid)
        {

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "UpdateFileContentGuidById",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("ContentGuid", DbType.String, 50, contentGuid.ToEmptyIfNull()),
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<Models.File> InsertUpdateAsync(Models.File model)
        {
            var id = await InsertUpdateInternal(
                model.Id,
                model.FeatureId,
                model.Name,
                model.Alias,
                model.Extension,
                model.ContentBlob,
                model.ContentType,
                model.ContentLength,
                model.ContentGuid,
                model.ContentCheckSum,
                model.TotalViews,
                model.CreatedUserId,
                model.CreatedDate,
                model.ModifiedUserId,
                model.ModifiedDate);

            if (id > 0)
                return await SelectByIdAsync(id);

            return null;
        }

        public async Task<File> SelectByIdAsync(int id)
        {
            Models.File file = null;
            using (var context = _dbContext)
            {
                file = await context.ExecuteReaderAsync<Models.File>(
                    CommandType.StoredProcedure,
                    "SelectFileById",
                    async reader =>
                    {
                        if ((reader != null) && reader.HasRows)
                        {
                            await reader.ReadAsync();
                            file = new Models.File();
                            file.PopulateModel(reader);
                        }

                        return file;
                    }, new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });

            }

            return file;
        }

        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            int featureId,
            string name,
            string alias,
            string extension,
            byte[] contentBlob,
            string contentType,
            long contentLength,
            string contentGuid,
            string contentCheckSum,
            int totalViews,
            int createdUserId,
            DateTimeOffset? createdDate,
            int modifiedUserId,
            DateTimeOffset? modifiedDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateFile",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("FeatureId", DbType.Int32, featureId),
                        new DbParam("Name", DbType.String, 255, name.ToSafeFileName().ToEmptyIfNull()),
                        new DbParam("Alias", DbType.String, 255, alias.ToEmptyIfNull()),
                        new DbParam("Extension", DbType.String, 16, extension.ToEmptyIfNull()),
                        new DbParam("ContentBlob", DbType.Binary, contentBlob ?? new byte[0]),
                        new DbParam("ContentType", DbType.String, 75, contentType.ToEmptyIfNull()),
                        new DbParam("ContentLength", DbType.Int64, contentLength),
                        new DbParam("ContentGuid", DbType.String, 32, contentGuid.ToEmptyIfNull()),
                        new DbParam("ContentCheckSum", DbType.String, 32, contentCheckSum.ToEmptyIfNull()),
                        new DbParam("TotalViews", DbType.Int32, totalViews),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });

            }

            return output;

        }

        #endregion

    }

}
