using System.Threading.Tasks;

namespace PlatoCore.Tasks.Abstractions
{

    public interface IBackgroundTaskProvider
    {

        int IntervalInSeconds { get; }

        Task ExecuteAsync(object sender, SafeTimerEventArgs args);

    }

}
