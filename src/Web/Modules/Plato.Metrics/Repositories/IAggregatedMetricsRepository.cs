using System;
using System.Threading.Tasks;
using PlatoCore.Models.Metrics;
using PlatoCore.Repositories.Metrics;

namespace Plato.Metrics.Repositories
{
    public interface IAggregatedMetricsRepository : IAggregatedRepository
    {

        Task<AggregatedResult<string>> SelectGroupedByFeatureAsync(DateTimeOffset start, DateTimeOffset end);

        Task<AggregatedResult<string>> SelectGroupedByRoleAsync(DateTimeOffset start, DateTimeOffset end);

        Task<AggregatedResult<string>> SelectGroupedByStringAsync(
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end,
            int limit = 20);

    }

}
