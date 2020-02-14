using PlatoCore.Abstractions;

namespace PlatoCore.Models
{
    public class BaseDocument : Serializable, IDocument
    {

        public int Id { get; set; }

    }

}
