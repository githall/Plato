using PlatoCore.Abstractions;

namespace Plato.Follows.Services
{
    public interface IFollowManager<TFollow> : ICommandManager<TFollow> where TFollow : class
    {
    }

}
