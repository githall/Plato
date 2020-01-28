using System;
using System.Threading.Tasks;
using PlatoCore.Models.Metrics;

namespace PlatoCore.Repositories.Metrics
{
    public interface IAggregatedUserRepository : IAggregatedRepository
    {

        Task<AggregatedResult<string>> SelectUserMetricsAsync(DateTimeOffset start, DateTimeOffset end);
        
    }

}
