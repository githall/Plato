using System;
using System.Threading.Tasks;
using PlatoCore.Models.Metrics;

namespace PlatoCore.Repositories.Metrics
{
    public interface IAggregatedRepository
    {

        Task<AggregatedResult<DateTimeOffset>> SelectGroupedByDateAsync(
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end);

    }


}
