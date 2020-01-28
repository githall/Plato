using System.Collections.Generic;

namespace PlatoCore.Search.Abstractions
{

    public interface IFullTextIndexProvider
    {
        IEnumerable<FullTextIndex> GetIndexes();
    }
    
}
