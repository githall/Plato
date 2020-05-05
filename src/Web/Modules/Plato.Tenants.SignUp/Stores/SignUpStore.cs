﻿using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Tenants.SignUp.Models;
using Plato.Tenants.SignUp.Repositories;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;

namespace Plato.Tenants.SignUp.Stores
{

    public class SignUpStore : ISignUpStore<Models.SignUp>
    {
        public const string ById = "ById";
        public const string BySessionId = "BySessionId";

        private readonly ISignUpRepository<Models.SignUp> _signUpRepository;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<SignUpStore> _logger;
       
        public SignUpStore(
            ISignUpRepository<Models.SignUp> signUpRepository, 
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager,
            ILogger<SignUpStore> logger)
        {
            _signUpRepository = signUpRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }

        #region "Implementation"

        public async Task<Models.SignUp> CreateAsync(Models.SignUp model)
        {
            var result = await _signUpRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<Models.SignUp> UpdateAsync(Models.SignUp model)
        {
            var result = await _signUpRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<bool> DeleteAsync(Models.SignUp model)
        {

            var success = await _signUpRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted sign-up '{0}' with id {1}",
                        model.Email, model.Id);
                }

                CancelTokens(model);

            }

            return success;

        }

        public async Task<Models.SignUp> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ById, id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _signUpRepository.SelectByIdAsync(id));
        }

        public async Task<Models.SignUp> GetBySessionIdAsync(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentNullException(nameof(sessionId));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), BySessionId, sessionId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _signUpRepository.SelectBySessionIdAsync(sessionId));

        }

        public IQuery<Models.SignUp> QueryAsync()
        {
            var query = new SignUpQuery(this);
            return _dbQuery.ConfigureQuery<Models.SignUp>(query); ;
        }

        public async Task<IPagedResults<Models.SignUp>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting sign-ups for key '{0}' with the following parameters: {1}",
                        token.ToString(), dbParams.Select(p => p.Value));
                }

                return await _signUpRepository.SelectAsync(dbParams);
              
            });
        }

        public void CancelTokens(Models.SignUp model = null)
        {
            _cacheManager.CancelTokens(this.GetType());
        }

        #endregion

    }

}
