using System.Data;

namespace PlatoCore.Data.Tracing.Abstractions
{

    public interface IDbTraceCommand
    {

        string CommandText { get; }

        CommandType CommandType { get; }

        IDbDataParameter[] Parameters { get; }

    }

    public class DbTraceCommand : IDbTraceCommand
    {

        public DbTraceCommand(string commandText, CommandType commandType, IDbDataParameter[] parameteres)
        {
            CommandText = commandText;
            CommandType = commandType;
            Parameters = parameteres;
        }

        public string CommandText { get; private set; }

        public CommandType CommandType { get; private set; }

        public IDbDataParameter[] Parameters { get; private set; }

    }

}
