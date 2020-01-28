using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Tasks.Abstractions;

namespace PlatoCore.Tasks
{
    public class SingletonTaskState : IDeferredTaskState
    {
        public IList<Func<DeferredTaskContext, Task>> Tasks { get; } = new List<Func<DeferredTaskContext, Task>>();

    }
}
