using System.Threading.Tasks;

namespace PlatoCore.Localization.Abstractions
{
    public interface ILocaleWatcher
    {
        Task WatchForChanges();

    }

}
