using System;
using PlatoCore.Hosting.Abstractions;

namespace PlatoCore.Hosting.Web
{
    
    public class CapturedHttpContext : ICapturedHttpContext
    {
        private readonly CapturedHttpContextState _state;

        public CapturedHttpContextState State => _state;

        public CapturedHttpContext()
        {
            _state = new CapturedHttpContextState();
        }

        public ICapturedHttpContext Configure(Action<CapturedHttpContextState> configure)
        {
            configure(_state);
            return this;
        }

    }
    
}
