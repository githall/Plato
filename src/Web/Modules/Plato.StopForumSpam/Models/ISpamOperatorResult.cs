using PlatoCore.Abstractions;

namespace Plato.StopForumSpam.Models
{
    public interface ISpamOperatorResult<out TModel> : ICommandResultBase where TModel : class
    {
        TModel Response { get; }

        ISpamOperation Operation { get; }

    }

}
