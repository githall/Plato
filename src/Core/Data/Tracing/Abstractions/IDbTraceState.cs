using System.Collections.Generic;

namespace PlatoCore.Data.Tracing.Abstractions
{

    public interface IDbTraceState
    {

        ICollection<IDbTrace> Traces { get; }

        IDbTrace AddOrUpdate(IDbTrace trace);

    }

}
