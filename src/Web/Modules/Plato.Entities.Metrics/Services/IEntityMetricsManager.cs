using PlatoCore.Abstractions;

namespace Plato.Entities.Metrics.Services
{
    public interface IEntityMetricsManager<TReaction> : ICommandManager<TReaction> where TReaction : class
    {
    }

}
