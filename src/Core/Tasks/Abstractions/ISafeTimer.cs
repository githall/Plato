using System;
using System.Threading.Tasks;

namespace PlatoCore.Tasks.Abstractions
{


    public delegate void TimerEventHandler(object sender, SafeTimerEventArgs e);
    
    public interface ISafeTimer
    {
        SafeTimerOptions Options { get; set; }
        
        event TimerEventHandler Elapsed;

        //Func<object, SafeTimerEventArgs, Task> Elapsed { get; set; }

        void Start();

        void Stop();

        void WaitToStop();

    }

}
