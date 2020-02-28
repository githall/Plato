using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;
using Plato.Entities.Attachments.Models;

namespace Plato.Entities.Attachments.Repositories
{

    public class EntityAttachmentRepository : IEntityAttachmentRepository<EntityAttachment>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityAttachmentRepository> _logger;

        public EntityAttachmentRepository(
            IDbContext dbContext,
            ILogger<EntityAttachmentRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<EntityAttachment> InsertUpdateAsync(EntityAttachment model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var id = await InsertUpdateInternal(
                model.Id,
                model.EntityId,
                model.AttachmentId,
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

        public async Task<EntityAttachment> SelectByIdAsync(int id)
        {
            EntityAttachment output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityAttachmentById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            output = new EntityAttachment();
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

        public async Task<IPagedResults<EntityAttachment>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<EntityAttachment> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<EntityAttachment>>(
                    CommandType.StoredProcedure,
                    "SelectEntityAttachmentsPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<EntityAttachment>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new EntityAttachment();
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
                _logger.LogInformation($"Deleting entity attachment relationship with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityAttachmentById",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<IEnumerable<EntityAttachment>> SelectByEntityIdAsync(int entityId)
        {
            IList<EntityAttachment> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IList<EntityAttachment>>(
                    CommandType.StoredProcedure,
                    "SelectEntityAttachmentsByEntityId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<EntityAttachment>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new EntityAttachment();
                                entity.PopulateModel(reader);
                                output.Add(entity);
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
                _logger.LogInformation($"Deleting all entity attachment relationships for entity id: {entityId}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityAttachmentsByEntityId",
                    new IDbDataParameter[]
                    {
                        new DbParam("EntityId", DbType.Int32, entityId)
                    });
            }

            return success > 0 ? true : false;
        }

        public async Task<bool> DeleteByEntityIdAndAttachmentIdAsync(int entityId, int attachmentId)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(
                    $"Deleting entity attachment relationship with entityId '{entityId}' and attachmentId '{attachmentId}'");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityAttachmentByEntityIdAndAttachmentId",
                    new IDbDataParameter[]
                    {
                        new DbParam("EntityId", DbType.Int32, entityId),
                        new DbParam("AttachmentId", DbType.Int32, attachmentId)
                    });
            }

            return success > 0 ? true : false;
        }

        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            int entityId,
            int attachmentId,
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
                    "InsertUpdateEntityAttachment",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("EntityId", DbType.Int32, entityId),
                        new DbParam("AttachmentId", DbType.Int32, attachmentId),
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
