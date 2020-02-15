using PlatoCore.Data.Tracing.Abstractions;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PlatoCore.Data.Tracing
{

    public class DefaultDbTraceState : IDbTraceState
    {

        private int _traceId;

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

            // Use existing trace id or generate a new one
            _traceId = trace.Id == 0
                ? _traceId++
                : trace.Id;

            return _traces.AddOrUpdate(_traceId, trace, (k, v) =>
            {
                return trace;
            });

        }

    }

}
