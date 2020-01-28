using System;

namespace PlatoCore.Tasks.Abstractions
{

    public interface IBackgroundTaskManager
    {
        
        void StartTasks();

        void StopTasks();

    }

}
