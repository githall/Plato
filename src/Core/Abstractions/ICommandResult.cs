using System.Collections.Generic;

namespace PlatoCore.Abstractions
{

    public interface ICommandResultBase
    {

        bool Succeeded { get; }

        IEnumerable<CommandError> Errors { get; }

    }

    public interface ICommandResult<out TResponse> : ICommandResultBase where TResponse : class
    {

        TResponse Response { get; }

    }

}
