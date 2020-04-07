using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Plato.Files.Sharing.Models;
using PlatoCore.Data.Abstractions;

namespace Plato.Files.Sharing.Repositories
{

    class FileInviteRepository : IFileInviteRepository<FileInvite>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<FileInviteRepository> _logger;

        public FileInviteRepository(
            IDbContext dbContext,
            ILogger<FileInviteRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        
        #region "Implementation"

        public async Task<FileInvite> InsertUpdateAsync(FileInvite model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                throw new ArgumentNullException(nameof(model.Email));
            }

            var id = await InsertUpdateInternal(
                model.Id,
                model.FileId,
                model.Email,
                model.CreatedUserId,
                model.CreatedDate);

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<FileInvite> SelectByIdAsync(int id)
        {
            FileInvite invite = null;
            using (var context = _dbContext)
            {
                invite = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectFileInviteById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            invite = new FileInvite();
                            invite.PopulateModel(reader);
                        }

                        return invite;
                    }, new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });

            }

            return invite;
        }

        public async Task<IPagedResults<FileInvite>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<FileInvite> output = null;
            using (var context = _dbContext)
            {

                output = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectFileInvitesPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<FileInvite>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new FileInvite();
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
                    dbParams);

             
            }

            return output;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting file invite with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteFileInviteById",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;
        }

        public async Task<IEnumerable<FileInvite>> SelectByEmailAndFileIdAsync(string email, int fileId)
        {
            IList<FileInvite> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectFileInvitesByEmailAndFileId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<FileInvite>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new FileInvite();
                                entity.PopulateModel(reader);
                                output.Add(entity);
                            }

                        }

                        return output;
                    }, new IDbDataParameter[]
                    {
                        new DbParam("Email", DbType.String, 255, email),
                        new DbParam("FileId", DbType.Int32, fileId)
                    });

            }

            return output;
        }

        public async Task<FileInvite> SelectByEmailFileIdAndCreatedUserIdAsync(string email, int fileId, int createdUserId)
        {
            FileInvite invite = null;
            using (var context = _dbContext)
            {
                invite = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectFileInviteByEmailFileIdAndCreatedUserId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            invite = new FileInvite();
                            invite.PopulateModel(reader);
                        }

                        return invite;

                    }, new IDbDataParameter[]
                    {
                        new DbParam("Email", DbType.String, 255, email),
                        new DbParam("FileId", DbType.Int32, fileId),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                    });
                
            }

            return invite;
        }

        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,            
            int fileId,
            string email,
            int createdUserId,
            DateTimeOffset createdDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateFileInvite",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("FileId", DbType.Int32, fileId),
                        new DbParam("Email", DbType.String, 255, email),                                                
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output)
                    });
            }

            return output;

        }

        #endregion
        
    }

}
