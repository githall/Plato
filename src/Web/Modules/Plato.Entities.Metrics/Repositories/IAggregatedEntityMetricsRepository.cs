using System;
using System.Threading.Tasks;
using PlatoCore.Models.Metrics;
using PlatoCore.Repositories.Metrics;

namespace Plato.Entities.Metrics.Repositories
{

    public interface IAggregatedEntityMetricsRepository : IAggregatedRepository
    {

        Task<AggregatedResult<DateTimeOffset>> SelectGroupedByDateAsync(
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end,
            int featureId);

        Task<AggregatedResult<int>> SelectGroupedByIntAsync(
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end);

        Task<AggregatedResult<int>> SelectGroupedByIntAsync(
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end,
            int featureId);

    }
}
