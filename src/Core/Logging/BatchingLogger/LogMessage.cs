using System;
using System.Collections.Generic;
using System.Text;

namespace PlatoCore.Logging.BatchingLogger
{
    public struct LogMessage
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Message { get; set; }
    }
}
