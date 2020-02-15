using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.Models;
using PlatoCore.Abstractions;
using PlatoCore.Data.Abstractions;

namespace Plato.Entities.Repositories
{

    public class SimpleEntityRepository<TModel> : ISimpleEntityRepository<TModel> where TModel : class, ISimpleEntity
    {

        private readonly ILogger<SimpleEntityRepository<TModel>> _logger;
        private readonly IDbContext _dbContext;

        public SimpleEntityRepository(  
            ILogger<SimpleEntityRepository<TModel>> logger,
            IDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<TModel> SelectByIdAsync(int id)
        {

            TModel entity = null;
            using (var context = _dbContext)
            {
                entity = await context.ExecuteReaderAsync<TModel>(
                    CommandType.StoredProcedure,
                    "SelectSimpleEntityById",
                    async reader => await BuildFromResultSets(reader),
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return entity;

        }
        
        public async Task<IPagedResults<TModel>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<TModel> results = null;
            using (var context = _dbContext)
            {
                results = await context.ExecuteReaderAsync<IPagedResults<TModel>>(
                    CommandType.StoredProcedure,
                    "SelectEntitiesPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            var output = new PagedResults<TModel>();
                            while (await reader.ReadAsync())
                            {
                                var entity = ActivateInstanceOf<TModel>.Instance();
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

                            return output;
                        }

                        return null;

                    },
                    dbParams);
               
            }

            return results;
        }

        public async Task<IEnumerable<TModel>> SelectByFeatureIdAsync(int featureId)
        {
            IList<TModel> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IList<TModel>>(
                    CommandType.StoredProcedure,
                    "SelectSimpleEntitiesByFeatureId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<TModel>();
                            while (await reader.ReadAsync())
                            {
                                var entity = ActivateInstanceOf<TModel>.Instance();
                                entity.PopulateModel(reader);
                                output.Add(entity);
                            }
                        }

                        return output;

                    }, new IDbDataParameter[]
                    {
                        new DbParam("FeatureId", DbType.Int32, featureId)
                    });

            }

            return output;

        }

        // -----------------

        async Task<TModel> BuildFromResultSets(DbDataReader reader)
        {

            TModel model = null;
            if ((reader != null) && (reader.HasRows))
            {
                model = ActivateInstanceOf<TModel>.Instance();
                await reader.ReadAsync();
                model.PopulateModel(reader);
            }

            return model;

        }

    }

}