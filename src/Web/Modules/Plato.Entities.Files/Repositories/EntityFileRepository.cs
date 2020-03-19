using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;
using Plato.Entities.Files.Models;

namespace Plato.Entities.Files.Repositories
{

    public class EntityFileRepository : IEntityFileRepository<EntityFile>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityFileRepository> _logger;

        public EntityFileRepository(
            IDbContext dbContext,
            ILogger<EntityFileRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<EntityFile> InsertUpdateAsync(EntityFile model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var id = await InsertUpdateInternal(
                model.Id,
                model.EntityId,
                model.FileId,
                model.CreatedUserId,
                model.CreatedDate,
                model.ModifiedUserId,
                model.ModifiedDate);

            if (id > 0)
            {
                return await SelectByIdAsync(id);
            }

            return null;

        }

        public async Task<EntityFile> SelectByIdAsync(int id)
        {
            EntityFile output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityFileById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            output = new EntityFile();
                            output.PopulateModel(reader);
                        }

                        return output;
                    }, new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });


            }

            return output;

        }

        public async Task<IPagedResults<EntityFile>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<EntityFile> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<EntityFile>>(
                    CommandType.StoredProcedure,
                    "SelectEntityFilesPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<EntityFile>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new EntityFile();
                                entity.PopulateModel(reader);
                                output.Data.Add(entity);
                            }

                            if (await reader.NextResultAsync())
                            {
                                if (reader.HasRows)
                                {
                                    await reader.ReadAsync();
                                    output.PopulateTotal(reader);
                                }
                            }

                        }

                        return output;
                    },
                    dbParams
                );

            }

            return output;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting entity file relationship with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityFileById",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<IEnumerable<EntityFile>> SelectByEntityIdAsync(int entityId)
        {
            IList<EntityFile> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IList<EntityFile>>(
                    CommandType.StoredProcedure,
                    "SelectEntityFilesByEntityId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<EntityFile>();
                            while (await reader.ReadAsync())
                            {
                                var entityFile = new EntityFile();
                                entityFile.PopulateModel(reader);
                                output.Add(entityFile);
                            }

                        }

                        return output;
                    }, new IDbDataParameter[]
                    {
                        new DbParam("EntityId", DbType.Int32, entityId)
                    });

            }

            return output;
        }

        public async Task<bool> DeleteByEntityIdAsync(int entityId)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting all entity file relationships for entity id: {entityId}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityFilesByEntityId",
                    new IDbDataParameter[]
                    {
                        new DbParam("EntityId", DbType.Int32, entityId)
                    });
            }

            return success > 0 ? true : false;
        }

        public async Task<bool> DeleteByFileIdAsync(int fileId)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting all entity file relationships for file id: {fileId}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityFilesByFileId",
                    new IDbDataParameter[]
                    {
                        new DbParam("FileId", DbType.Int32, fileId)
                    });
            }

            return success > 0 ? true : false;
        }


        public async Task<bool> DeleteByEntityIdAndFileIdAsync(int entityId, int fileId)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(
                    $"Deleting entity file relationship with entityId '{entityId}' and fileId '{fileId}'");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityFileByEntityIdAndFileId",
                    new IDbDataParameter[]
                    {
                        new DbParam("EntityId", DbType.Int32, entityId),
                        new DbParam("FileId", DbType.Int32, fileId)
                    });
            }

            return success > 0 ? true : false;
        }

        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            int entityId,
            int fileId,
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
                    "InsertUpdateEntityFile",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("EntityId", DbType.Int32, entityId),
                        new DbParam("FileId", DbType.Int32, fileId),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output)
                    });
            }

            return output;

        }

        #endregion

    }

}
