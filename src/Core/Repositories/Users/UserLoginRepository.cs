using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Models.Users;

namespace PlatoCore.Repositories.Users
{

    public class UserLoginRepository : IUserLoginRepository<UserLogin>
    {

        #region "Constructor"

        private readonly ILogger<UserLoginRepository> _logger;
        private readonly IDbContext _dbContext;

        public UserLoginRepository(
            ILogger<UserLoginRepository> logger,
            IDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region "Implementation"

        public async Task<bool> DeleteAsync(int id)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting user login with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteUserLoginById",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<UserLogin> InsertUpdateAsync(UserLogin model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var id = await InsertUpdateInternal(
                model.Id,
                model.UserId,
                model.LoginProvider,
                model.ProviderKey,
                model.ProviderDisplayName,                
                model.CreatedDate);

            if (id > 0)
            {
                return await SelectByIdAsync(id);
            }

            return null;

        }

        public async Task<IPagedResults<UserLogin>> SelectAsync(IDbDataParameter[] dbParams)
        {

            IPagedResults<UserLogin> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserLoginsPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<UserLogin>();
                            while (await reader.ReadAsync())
                            {
                                var userLogin = new UserLogin();
                                userLogin.PopulateModel(reader);
                                output.Data.Add(userLogin);
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

        public async Task<UserLogin> SelectByIdAsync(int id)
        {

            UserLogin userLogin = null;
            using (var context = _dbContext)
            {
                userLogin = await context.ExecuteReaderAsync<UserLogin>(
                    CommandType.StoredProcedure,
                    "SelectUserLoginById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            userLogin = new UserLogin();
                            await reader.ReadAsync();
                            userLogin.PopulateModel(reader);
                        }

                        return userLogin;
                    }, new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });

            }

            return userLogin;

        }

        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            int userId,
            string loginPrrovider,
            string providerKey,
            string providerDisplayName,            
            DateTimeOffset? createdDate)
        {
            
            using (var context = _dbContext)
            {
                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateUserLogin",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("UserId", DbType.Int32, userId),
                        new DbParam("LoginProvider", DbType.String, 128, loginPrrovider.ToEmptyIfNull()),
                        new DbParam("ProviderKey", DbType.String, 128, providerKey.ToEmptyIfNull()),
                        new DbParam("ProviderDisplayName", DbType.String, providerDisplayName.ToEmptyIfNull()),                                                
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),                        
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });
            }
        }

        #endregion

    }

}
