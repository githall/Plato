using System;

namespace PlatoCore.Tasks.Abstractions
{
    public interface ISafeTimerFactory
    {

        void Start(Action<object, SafeTimerEventArgs> action, SafeTimerOptions options);

        void Stop();

    }

}
