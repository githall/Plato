using System.Data;

namespace PlatoCore.Data.Tracing.Abstractions
{
    public interface IDbTracer<TProvider> where TProvider : class
    {

        bool Enabled { get; }

        void Start(string commandText, CommandType commandType, IDbDataParameter[] dbParams);

        void Stop();

    }

}
