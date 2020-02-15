using System;

namespace PlatoCore.Data.Tracing.Abstractions
{

    public interface IDbTrace
    {

        int Id { get; set; }

        IDbTraceCommand Command { get; set; }

        long StartTime { get; set; }

        long EndTime { get; set; }

        long ExecutionTime { get; }

    }

    public class DbTrace : IDbTrace
    {

        public int Id { get; set; }

        public IDbTraceCommand Command { get; set; }

        public long StartTime { get; set; }

        public long EndTime { get; set; }

        public long ExecutionTime => EndTime - StartTime / 10000000;

        public DbTrace()
        {
            StartTime = DateTime.Now.Ticks;
        }

        public DbTrace(IDbTraceCommand command) : this()
        {
            Command = command;
        }

    }

}
