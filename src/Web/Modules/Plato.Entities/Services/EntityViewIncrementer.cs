using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Net;
using PlatoCore.Net.Abstractions;

namespace Plato.Entities.Services
{
    
    public class EntityViewIncrementer<TEntity> : IEntityViewIncrementer<TEntity> where TEntity : class, IEntity
    {

        public const string CookieName = "plato_reads";
        private HttpContext _context;

        private readonly IEntityStore<TEntity> _entityStore;
        private readonly ICookieBuilder _cookieBuilder;
        private readonly IDbHelper _dbHelper;    

        public EntityViewIncrementer(
            IEntityStore<TEntity> entityStore,         
            ICookieBuilder cookieBuilder,
            IDbHelper dbHelper)
        {
            _cookieBuilder = cookieBuilder;
            _entityStore = entityStore;        
            _dbHelper = dbHelper;
        }

        public IEntityViewIncrementer<TEntity> Contextulize(HttpContext context)
        {
            _context = context;
            return this;
        }

        public async Task<TEntity> IncrementAsync(TEntity entity)
        {
            
            // Transform tracking cookie into int array
            List<int> values = null;

            var storage = "";
            if (_context != null)
            {
                storage = _context.Request.Cookies[CookieName];
            }
          
            // Read existing into values
            if (!String.IsNullOrEmpty(storage))
            {
                values = storage.ToIntArray().ToList();
            }

            // Does the entity Id we are accessing exist in our store
            if (values != null)
            {
                if (values.Contains(entity.Id))
                {
                    return entity;
                }
            }

            await UpdateTotalViewsAsync(entity);


            if (values == null)
            {
                values = new List<int>();
            }

            values.Add(entity.Id);

            // If a context is supplied use a client side cookie to track views
            // Expire the cookie every 10 minutes using a sliding expiration to
            // ensure views are updated often but not on every refresh
            _cookieBuilder
                .Contextulize(_context)
                .Append(CookieName, values.ToArray().ToDelimitedString(),
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = DateTime.Now.AddMinutes(10)
                    });

            return entity;

        }

        async Task UpdateTotalViewsAsync(TEntity entity)
        {
            // Sql query
            const string sql = "UPDATE {prefix}_Entities SET TotalViews = {views} WHERE Id = {id};";

            // Execute and return results
            await _dbHelper.ExecuteScalarAsync<int>(sql, new Dictionary<string, string>()
            {
                ["{id}"] = entity.Id.ToString(),
                ["{views}"] = (entity.TotalViews += 1).ToString()
            });

            _entityStore.CancelTokens(entity);
        }

    }

}
