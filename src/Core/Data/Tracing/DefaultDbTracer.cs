using System;
using System.Data;
using Microsoft.Extensions.Hosting;
using PlatoCore.Data.Tracing.Abstractions;

namespace PlatoCore.Data.Tracing
{

    public class DefaultDbTracer<TProvider> : IDbTracer<TProvider> where TProvider : class
    {

        private IDbTrace _currentTrace;

        public bool Enabled { get; }

        private readonly IDbTraceState _dbTraceState;

        public DefaultDbTracer(
            IHostEnvironment hostEnvironment,
            IDbTraceState dbTraceState)
        {
            // Enabling profiling by default within development
            if (hostEnvironment.IsDevelopment())
            {
                Enabled = true;
            }

            _dbTraceState = dbTraceState;

        }

        public void Start(string commandText, CommandType commandType, IDbDataParameter[] dbParams)
        {

            if (!Enabled)
            {
                return;
            }

            var command = new DbTraceCommand(commandText, commandType, dbParams);
            _currentTrace = _dbTraceState.AddOrUpdate(new DbTrace(command));            

        }

        public void Stop()
        {

            if (!Enabled)
            {
                return;
            }

            _currentTrace.EndTime = DateTime.Now.Ticks;
            _currentTrace.ElapsedTime = TimeSpan.FromMilliseconds(_currentTrace.StartTime);            
            _dbTraceState.AddOrUpdate(_currentTrace);

        }

    }

}
