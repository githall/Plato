using PlatoCore.Abstractions;

namespace Plato.Questions.Services
{
    public interface IPostManager<TEntity> : ICommandManager<TEntity> where TEntity : class
    {
    }

}
