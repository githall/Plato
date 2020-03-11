using System.Collections.Generic;
using PlatoCore.Search.Abstractions;

namespace Plato.Attachments
{

    public class FullTextIndexes : IFullTextIndexProvider
    {

        public static readonly FullTextIndex Attachments =
            new FullTextIndex("Attachments", new List<FullTextColumn>()
            {
                new FullTextColumn("Name"),
                new FullTextColumn("ContentBlob", "Extension")
            });

        public IEnumerable<FullTextIndex> GetIndexes()
        {
            return new[]
            {
                Attachments
            };
        }

    }

}
