using PlatoCore.Data.Tracing.Abstractions;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PlatoCore.Data.Tracing
{

    public class DefaultDbTraceState : IDbTraceState
    {

        private int _traceId = 1;

        private ConcurrentDictionary<int, IDbTrace> _traces;

        public ICollection<IDbTrace> Traces
        {
            get
            {
                if (_traces == null)
                {
                    return null;
                }
                return _traces.Values;
            }
        }

        public IDbTrace AddOrUpdate(IDbTrace trace)
        {

            if (_traces == null)
            {
                _traces = new ConcurrentDictionary<int, IDbTrace>();
            }

            // Initialize new traces with a new id
            if (trace.Id == 0)
            {
                trace.Id = _traceId;
                _traceId++;
            }

            return _traces.AddOrUpdate(trace.Id, trace, (k, v) =>
            {
                return trace;
            });

        }

    }

}
