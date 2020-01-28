using PlatoCore.Abstractions;

namespace Plato.Labels.Services
{
    public interface IEntityLabelManager<TEntityLabel> : ICommandManager<TEntityLabel> where TEntityLabel : class
    {

    }

}
