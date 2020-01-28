using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Models.Abstract;

namespace PlatoCore.Repositories.Abstract
{
    public class DictionaryRepository : IDictionaryRepository<DictionaryEntry>
    {

        private readonly ILogger<DictionaryRepository> _logger;
        private readonly IDbContext _dbContext;

        public DictionaryRepository(            
            ILogger<DictionaryRepository> logger,
            IDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        
        public async Task<bool> DeleteAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting dictionary entry with id: {id}");
            }
                
            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteDictionaryEntryById",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<DictionaryEntry> InsertUpdateAsync(DictionaryEntry dictionaryEntry)
        {
            var id = await InsertUpdateInternal(
                dictionaryEntry.Id,    
                dictionaryEntry.Key,
                dictionaryEntry.Value,
                dictionaryEntry.CreatedDate,
                dictionaryEntry.CreatedUserId,
                dictionaryEntry.ModifiedDate,
                dictionaryEntry.ModifiedUserId);
            if (id > 0)
                return await SelectByIdAsync(id);
            return null;
        }

        public async Task<DictionaryEntry> SelectByIdAsync(int id)
        {
     
            DictionaryEntry output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectDictionaryEntryById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new DictionaryEntry();
                            await reader.ReadAsync();
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

        public async Task<IEnumerable<DictionaryEntry>> SelectEntries()
        {
            IList<DictionaryEntry> output = null;
            if (_dbContext != null)
            {
                using (var context = _dbContext)
                {
                    output = await context.ExecuteReaderAsync<IList<DictionaryEntry>>(
                        CommandType.StoredProcedure,
                        "SelectDictionaryEntries",
                        async reader =>
                        {
                            if ((reader != null) && (reader.HasRows))
                            {
                                output = new List<DictionaryEntry>();
                                while (await reader.ReadAsync())
                                {
                                    var entry = new DictionaryEntry();
                                    entry.PopulateModel(reader);
                                    output.Add(entry);
                                }
                            }

                            return output;

                        });
                }
            }

            return output;

        }
        
        public async Task<DictionaryEntry> SelectEntryByKey(string key)
        {
            DictionaryEntry entry = null;
            if (_dbContext != null)
            {
                using (var context = _dbContext)
                {
                    entry = await context.ExecuteReaderAsync(
                        CommandType.StoredProcedure,
                        "SelectDictionaryEntryByKey",
                        async reader =>
                        {
                            if ((reader != null) && (reader.HasRows))
                            {
                                entry = new DictionaryEntry();
                                await reader.ReadAsync();
                                entry.PopulateModel(reader);
                            }

                            return entry;
                        }, new IDbDataParameter[]
                        {
                            new DbParam("Key", DbType.String, key)
                        });

                }
            }

            return entry;

        }

        public async Task<bool> DeleteByKeyAsync(string key)
        {

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting dicationary entry with key {key}.");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteDictionaryEntryByKey",
                    new IDbDataParameter[]
                    {
                        new DbParam("[Key]", DbType.String, key)
                    });
            }

            return success > 0 ? true : false;

        }

        public Task<IPagedResults<DictionaryEntry>> SelectAsync(IDbDataParameter[] dbParams)
        {
            // TODO
            throw new NotImplementedException();
        }

        // ------------

        async Task<int> InsertUpdateInternal(
            int id,   
            string key,
            string value,
            DateTimeOffset? createdDate,
            int createdUserId,
            DateTimeOffset? modifiedDate,
            int modifiedUserId)
        {

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }            

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(id == 0
                    ? $"Inserting dictionary entry with key: {key}"
                    : $"Updating dictionary entry with id: {id}");
            }

            var output = 0;
            using (var context = _dbContext)
            {
                if (context != null)
                {
                    output = await context.ExecuteScalarAsync<int>(
                        CommandType.StoredProcedure,
                        "InsertUpdateDictionaryEntry",
                        new IDbDataParameter[]
                        {
                            new DbParam("Id", DbType.Int32, id),
                            new DbParam("Key", DbType.String, 255, key),
                            new DbParam("Value", DbType.String, value.ToEmptyIfNull()),
                            new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                            new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                            new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                            new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate),
                            new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output)
                        });
                }
            }

            return output;

        }

    }

}
