using Microsoft.Extensions.DependencyInjection;
using System;

namespace PlatoCore.Tasks.Abstractions
{

    public interface IBackgroundTaskManager
    {
        
        void StartTasks();

        void StartTasks(IServiceProvider serviceProvider);

        void StopTasks();

    }

}
