using PlatoCore.Abstractions;

namespace PlatoCore.Reputations.Abstractions
{
    public interface IUserReputationManager<TUserReputation> : ICommandManager<TUserReputation> where TUserReputation : class
    {
    }

}
