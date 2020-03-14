using System.Collections.Generic;
using PlatoCore.Search.Abstractions;

namespace Plato.Files
{

    public class FullTextIndexes : IFullTextIndexProvider
    {

        public static readonly FullTextIndex Files =
            new FullTextIndex("Files", new List<FullTextColumn>()
            {
                new FullTextColumn("Name"),
                new FullTextColumn("ContentBlob", "Extension")
            });

        public IEnumerable<FullTextIndex> GetIndexes()
        {
            return new[]
            {
                Files
            };
        }

    }

}
