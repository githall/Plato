using PlatoCore.Data.Tracing.Abstractions;
using System.Collections.Generic;

namespace Plato.Data.Tracing.ViewModels
{
    public class TraceViewModel
    {

        public IEnumerable<IDbTrace> Traces { get; set; }
    }
}
