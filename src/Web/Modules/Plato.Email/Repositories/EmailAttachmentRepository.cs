using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Emails.Abstractions;

namespace Plato.Email.Repositories
{

    public class EmailAttachmentRepository : IEmailAttachmentRepository<EmailAttachment>
    {

        private readonly ILogger<EmailAttachmentRepository> _logger;
        private readonly IDbContext _dbContext;

        public EmailAttachmentRepository(            
            ILogger<EmailAttachmentRepository> logger,
            IDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        
        #region "Implementation"

        public async Task<IPagedResults<EmailAttachment>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<EmailAttachment> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<EmailAttachment>>(
                    CommandType.StoredProcedure,
                    "SelectEmailAttachmentsPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<EmailAttachment>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new EmailAttachment();
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
                _logger.LogInformation($"Deleting email attachment with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEmailAttachmentById",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;
        }
     
        public async Task<EmailAttachment> InsertUpdateAsync(EmailAttachment model)
        {
            var id = await InsertUpdateInternal(
                model.Id,
                model.EmailId,
                model.Name,
                model.Alias,
                model.Extension,
                model.ContentBlob,
                model.ContentType,
                model.ContentLength,          
                model.ContentCheckSum,           
                model.CreatedUserId,
                model.CreatedDate);

            if (id > 0)
                return await SelectByIdAsync(id);

            return null;
        }

        public async Task<EmailAttachment> SelectByIdAsync(int id)
        {
            EmailAttachment file = null;
            using (var context = _dbContext)
            {
                file = await context.ExecuteReaderAsync<EmailAttachment>(
                    CommandType.StoredProcedure,
                    "SelectEmailAttacmentById",
                    async reader =>
                    {
                        if ((reader != null) && reader.HasRows)
                        {
                            await reader.ReadAsync();
                            file = new EmailAttachment();
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

        public async Task<IEnumerable<EmailAttachment>> SelectByEmailIdAsync(int emailId)
        {

            IList<EmailAttachment> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IList<EmailAttachment>>(
                    CommandType.StoredProcedure,
                    "SelectEmailAttachmentsByEmailId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<EmailAttachment>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new EmailAttachment();
                                entity.PopulateModel(reader);
                                output.Add(entity);
                            }
                        }

                        return output;
                    }, new[]
                    {
                        new DbParam("EmailId", DbType.Int32, emailId)
                    });

            }

            return output;

        }

        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            int emailId,
            string name,
            string alias,
            string extension,
            byte[] contentBlob,
            string contentType,
            long contentLength,          
            string contentCheckSum,          
            int createdUserId,
            DateTimeOffset? createdDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEmailAttachment",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("EmailId", DbType.Int32, emailId),
                        new DbParam("Name", DbType.String, 255, name.ToSafeFileName().ToEmptyIfNull()),
                        new DbParam("Alias", DbType.String, 255, alias.ToEmptyIfNull()),
                        new DbParam("Extension", DbType.String, 16, extension.ToEmptyIfNull()),
                        new DbParam("ContentBlob", DbType.Binary, contentBlob ?? new byte[0]),
                        new DbParam("ContentType", DbType.String, 75, contentType.ToEmptyIfNull()),
                        new DbParam("ContentLength", DbType.Int64, contentLength),                        
                        new DbParam("ContentCheckSum", DbType.String, 32, contentCheckSum.ToEmptyIfNull()),                        
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),                       
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });

            }

            return output;

        }

        #endregion

    }

}
