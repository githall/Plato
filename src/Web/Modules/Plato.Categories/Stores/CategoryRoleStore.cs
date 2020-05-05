﻿using System.Linq;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Plato.Categories.Models;
using Plato.Categories.Repositories;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;

namespace Plato.Categories.Stores
{

    public class CategoryRoleStore : ICategoryRoleStore<CategoryRole>
    {

        private const string ByIdKey = "ById";
        private const string ByCategoryIdKey = "BycategoryId";

        private readonly ICategoryRoleRepository<CategoryRole> _categoryRoleRepository;
        private readonly ILogger<CategoryRoleStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;

        public CategoryRoleStore(
            ICategoryRoleRepository<CategoryRole> categoryRoleRepository,
            ILogger<CategoryRoleStore> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _categoryRoleRepository = categoryRoleRepository;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;            
        }

        public async Task<CategoryRole> CreateAsync(CategoryRole model)
        {
            var result = await _categoryRoleRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(model);
            }

            return result;
        }

        public async Task<CategoryRole> UpdateAsync(CategoryRole model)
        {
            var result = await _categoryRoleRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(model);
            }

            return result;

        }

        public async Task<bool> DeleteAsync(CategoryRole model)
        {

            var success = await _categoryRoleRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted category role for category '{0}' with id {1}",
                        model.CategoryId, model.Id);
                }
                CancelTokens(model);
            }

            return success;

        }

        public async Task<CategoryRole> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByIdKey, id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _categoryRoleRepository.SelectByIdAsync(id));

        }

        public IQuery<CategoryRole> QueryAsync()
        {
            var query = new CategoryRoleQuery(this);
            return _dbQuery.ConfigureQuery<CategoryRole>(query); ;
        }

        public async Task<IPagedResults<CategoryRole>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _categoryRoleRepository.SelectAsync(dbParams));
        }
      
        public async Task<IEnumerable<CategoryRole>> GetByCategoryIdAsync(int categoryId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByCategoryIdKey, categoryId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _categoryRoleRepository.SelectByCategoryIdAsync(categoryId));
        }

        public async Task<bool> DeleteByCategoryIdAsync(int categoryId)
        {
            
            var success = await _categoryRoleRepository.DeleteByCategoryIdAsync(categoryId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted all category roles for category '{0}'",
                        categoryId);
                }
                _cacheManager.CancelTokens(this.GetType());
                _cacheManager.CancelTokens(this.GetType(), ByCategoryIdKey, categoryId);
            
            }

            return success;

        }

        public async Task<bool> DeleteByRoleIdAndCategoryIdAsync(int roleId, int categoryId)
        {
            var success = await _categoryRoleRepository.DeleteByRoleIdAndCategoryIdAsync(roleId, categoryId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted category role for role '{0}' and category '{1}'",
                      roleId, categoryId);
                }
                _cacheManager.CancelTokens(this.GetType());
                _cacheManager.CancelTokens(this.GetType(), ByCategoryIdKey, categoryId);

            }

            return success;
        }

        public void CancelTokens(CategoryRole model)
        {
            _cacheManager.CancelTokens(this.GetType());
            _cacheManager.CancelTokens(this.GetType(), ByIdKey, model.Id);
            _cacheManager.CancelTokens(this.GetType(), ByCategoryIdKey, model.CategoryId);
        }

    }

}
