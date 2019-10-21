using Plato.Internal.Data.Abstractions;

namespace Plato.Tags.Models
{
    public class TagApiParams
    {

        public int Page { get; set; } = 1;

        public int Size { get; set; } = 10;

        public int FeatureId { get; set; }

        public string Keywords { get; set; }

        public string Sort { get; set; } = "TotalEntities";

        public OrderBy Order { get; set; } = OrderBy.Desc;

    }

}
