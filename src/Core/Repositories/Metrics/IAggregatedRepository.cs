using System;
using System.Threading.Tasks;
using PlatoCore.Data.Abstractions;
using PlatoCore.Models.Metrics;

namespace PlatoCore.Repositories.Metrics
{
    public interface IAggregatedRepository
    {

        public IDbHelper DbHelper { get; }

        Task<AggregatedResult<DateTimeOffset>> SelectGroupedByDateAsync(
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end);

    }


}
