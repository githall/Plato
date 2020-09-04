using System;

namespace PlatoCore.Data.Tracing.Abstractions
{

    public interface IDbTrace
    {

        int Id { get; set; }

        IDbTraceCommand Command { get; set; }

        long StartTime { get; set; }

        long EndTime { get; set; }

        TimeSpan ElapsedTime { get; set; }

    }

    public class DbTrace : IDbTrace
    {

        public int Id { get; set; }

        public IDbTraceCommand Command { get; set; }

        public long StartTime { get; set; }

        public long EndTime { get; set; }

        public TimeSpan ElapsedTime { get; set; }

        public DbTrace()
        {
            StartTime = DateTime.Now.Millisecond;
        }

        public DbTrace(IDbTraceCommand command) : this()
        {
            Command = command;
        }

    }

}
