using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Tenants.SignUp.Models;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;

namespace Plato.Tenants.SignUp.Repositories
{

    public class SignUpRepository : ISignUpRepository<Models.SignUp>
    {

        private readonly ILogger<SignUpRepository> _logger;
        private readonly IDbContext _dbContext;

        public SignUpRepository(            
            ILogger<SignUpRepository> logger,
            IDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        
        #region "Implementation"

        public async Task<IPagedResults<Models.SignUp>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<Models.SignUp> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<Models.SignUp>>(
                    CommandType.StoredProcedure,
                    "SelectSignUpsPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<Models.SignUp>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new Models.SignUp();
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
                _logger.LogInformation($"Deleting sign up with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteSignUpById",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;
        }

        public async Task<Models.SignUp> InsertUpdateAsync(Models.SignUp model)
        {
            var id = await InsertUpdateInternal(
                model.Id,
                model.SessionId,
                model.Email,
                model.CompanyName,
                model.CompanyNameAlias,
                model.EmailConfirmed,
                model.EmailUpdates,
                model.SecurityToken,
                model.CreatedDate);

            if (id > 0)
                return await SelectByIdAsync(id);

            return null;
        }

        public async Task<Models.SignUp> SelectByIdAsync(int id)
        {
            Models.SignUp output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<Models.SignUp>(
                    CommandType.StoredProcedure,
                    "SelectSignUpById",
                    async reader =>
                    {
                        if ((reader != null) && reader.HasRows)
                        {
                            await reader.ReadAsync();
                            output = new Models.SignUp();
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

        public async Task<Models.SignUp> SelectBySessionIdAsync(string sessionId)
        {

            Models.SignUp output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<Models.SignUp>(
                    CommandType.StoredProcedure,
                    "SelectSignUpBySessionId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            output = new Models.SignUp();
                            output.PopulateModel(reader);
                        }

                        return output;
                    }, new IDbDataParameter[]
                    {
                        new DbParam("SessionId", DbType.String, 255, sessionId)
                    });

            }

            return output;

        }

        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            string sessionId,
            string email,
            string companyName,
            string companyNameAlias,
            bool emailConfirmed,
            bool emailUpdates,
            string SecurityToken,
            DateTimeOffset? createdDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateSignUp",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("SessionId", DbType.String, 255, sessionId.ToEmptyIfNull()),
                        new DbParam("Email", DbType.String, 255, email.ToEmptyIfNull()),
                        new DbParam("CompanyName", DbType.String, 255, companyName.ToEmptyIfNull()),
                        new DbParam("CompanyNameAlias", DbType.String, 255, companyNameAlias.ToEmptyIfNull()),
                        new DbParam("EmailConfirmed", DbType.Boolean, emailConfirmed),
                        new DbParam("EmailUpdates", DbType.Boolean, emailUpdates),
                        new DbParam("SecurityToken", DbType.String, 8, SecurityToken.ToEmptyIfNull()),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),                        
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });

            }

            return output;

        }

        #endregion

    }

}
