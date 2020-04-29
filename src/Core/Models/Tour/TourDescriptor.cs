using PlatoCore.Abstractions;
using System.Collections.Generic;

namespace PlatoCore.Models.Tour
{

    public class TourDescriptor : Serializable
    {

        public bool Completed { get; set; }

        public IEnumerable<TourStep> Steps { get; set; }

    }

}
