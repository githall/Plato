using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Entities.Metrics.Repositories;
using PlatoCore.Abstractions.Extensions;

namespace Plato.Entities.Metrics.Extensions
{
    public static class AggregatedEntityMetricsRepositoryExtensions
    {
        
        public static async Task<IDictionary<int, DateTimeOffset?>> SelectMaxViewDateForEntitiesAsync(
            this IAggregatedEntityMetricsRepository repository,
            int userId,
            int[] entityIds)
        {

            const string sql = @"                
                SELECT 
                    em.EntityId AS EntityId, 
                    MAX(em.CreatedDate) AS CreatedDate
                FROM 
                     {prefix}_EntityMetrics em
                WHERE
                    em.CreatedUserId = {userId} AND em.EntityId IN ({entityIds})
                GROUP BY (em.EntityId)
            ";

            // Sql replacements
            var replacements = new Dictionary<string, string>()
            {
                ["{userId}"] = userId.ToString(),
                ["{entityIds}"] = entityIds.ToDelimitedString()
            };

            // Execute and return results
            return await repository.DbHelper.ExecuteReaderAsync(sql, replacements, async dr =>
            {
                var output = new Dictionary<int, DateTimeOffset?>();
                while (await dr.ReadAsync())
                {

                    var key = 0;
                    DateTimeOffset? value = null;

                    if (dr.ColumnIsNotNull("EntityId"))
                        key = Convert.ToInt32((dr["EntityId"]));

                    if (dr.ColumnIsNotNull("CreatedDate"))
                        value = (DateTimeOffset)(dr["CreatedDate"]);

                    if (!output.ContainsKey(key))
                    {
                        output[key] = value;
                    }

                }

                return output;
            });

        }

    }
}
