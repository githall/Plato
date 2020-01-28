using PlatoCore.Abstractions;

namespace Plato.Search.Commands
{
    public interface IFullTextIndexCommand<TIndex> : ICommandManager<TIndex> where TIndex : class
    {
    }
}
