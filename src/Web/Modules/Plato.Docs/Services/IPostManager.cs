using System.Threading.Tasks;
using Plato.Entities.Services;
using PlatoCore.Abstractions;

namespace Plato.Docs.Services
{
    public interface IPostManager<TEntity> : ICommandManager<TEntity> where TEntity : class
    {
        Task<ICommandResult<TEntity>> Move(TEntity model, MoveDirection direction);

    }

}
