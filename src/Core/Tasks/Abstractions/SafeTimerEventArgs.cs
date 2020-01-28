using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace PlatoCore.Tasks.Abstractions
{
    public class SafeTimerEventArgs : EventArgs
    {

        public IServiceProvider ServiceProvider { get; }

        public SafeTimerEventArgs()
        {
        }

        public SafeTimerEventArgs(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

    }
    
}
