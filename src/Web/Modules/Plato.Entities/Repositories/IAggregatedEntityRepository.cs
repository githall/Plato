using System;
using System.Threading.Tasks;
using PlatoCore.Models.Metrics;
using PlatoCore.Repositories.Metrics;

namespace Plato.Entities.Repositories
{

    public interface IAggregatedEntityRepository : IAggregatedRepository
    {

        Task<AggregatedResult<DateTimeOffset>> SelectGroupedByDateAsync(
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end,
            int featureId);

    }

}
