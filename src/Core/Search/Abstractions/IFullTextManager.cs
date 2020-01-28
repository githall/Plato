using System.Collections.Generic;

namespace PlatoCore.Search.Abstractions
{
    public interface IFullTextIndexManager
    {

        IEnumerable<FullTextIndex> GetIndexes();

        IDictionary<string, IEnumerable<FullTextIndex>> GetIndexesByTable();

    }
    
}
