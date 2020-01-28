using System.Collections.Generic;

namespace PlatoCore.Search.Abstractions
{

    public interface IFullTextQueryParser
    {

        HashSet<string> StopWords { get; set; }

        string ToFullTextSearchQuery(string query);

    }


}
