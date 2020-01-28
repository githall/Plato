using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Models.Users;
using PlatoCore.Repositories.Users;
using PlatoCore.Stores.Abstractions.Users;

namespace PlatoCore.Stores.Users
{

    class PlatoUserLoginStore : IPlatoUserLoginStore<UserLogin>
    {

        private readonly IUserLoginRepository<UserLogin> _userLoginRepository;
        private readonly ILogger<PlatoUserLoginStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;

        public PlatoUserLoginStore(
            IUserLoginRepository<UserLogin> userLoginRepository,
            ILogger<PlatoUserLoginStore> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _userLoginRepository = userLoginRepository;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;
        }

        public async Task<UserLogin> CreateAsync(UserLogin model)
        {

            var result = await _userLoginRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<UserLogin> UpdateAsync(UserLogin model)
        {

            var result = await _userLoginRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(model);
            }

            return result;

        }

        public async Task<bool> DeleteAsync(UserLogin model)
        {

            var result = await _userLoginRepository.DeleteAsync(model.Id);
            if (result)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"User login with id {model.Id} was deleted successfully");
                }
                CancelTokens(model);
            }

            return result;

        }

        public async Task<UserLogin> GetByIdAsync(int id)
        {

            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _userLoginRepository.SelectByIdAsync(id));

        }

        public IQuery<UserLogin> QueryAsync()
        {
            var query = new UserLoginQuery(this);
            return _dbQuery.ConfigureQuery<UserLogin>(query); ;
        }

        public async Task<IPagedResults<UserLogin>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _userLoginRepository.SelectAsync(dbParams));
        }

        public void CancelTokens(UserLogin model)
        {

            _cacheManager.CancelTokens(this.GetType());

            if (model != null)
            {
                _cacheManager.CancelTokens(this.GetType(), model.Id);                
            }
        }

    }

}
